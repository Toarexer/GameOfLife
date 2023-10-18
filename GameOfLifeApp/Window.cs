using Sim = GameOfLifeSim;
using GameOfLifeLogger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GameOfLifeApp;

class GameManagerWindow : Gtk.Window {
    public Sim.GameManager GameManager { get; private set; }

    private readonly Gtk.Paned _hPaned = new(Gtk.Orientation.Horizontal);
    private readonly Gtk.Paned _lvPaned = new(Gtk.Orientation.Vertical);
    private readonly Gtk.Paned _rvPaned = new(Gtk.Orientation.Vertical);

    private readonly Gtk.ScrolledWindow _typeListScroll = new();
    private readonly Gtk.ListBox _typeList = new();
    private readonly Gtk.ListBox _controlList = new();

    private readonly Gtk.DrawingArea _area = new();

    private readonly Gtk.Notebook _logTabs = new();
    private readonly Gtk.Label _infoTabLabel = new() { Text = "Info (0)" };
    private readonly Gtk.Label _errorTabLabel = new() { Text = "Errors (0)" };
    private readonly Gtk.ScrolledWindow _infoListScroll = new();
    private readonly Gtk.ListBox _infoList = new();
    private readonly Gtk.ScrolledWindow _errorListScroll = new();
    private readonly Gtk.ListBox _errorList = new();

    private int _updateInterval;
    private int _infoCount;
    private int _errorCount;

    public GameManagerWindow(string title, Sim.GameManager gm) : base(title) {
        DefaultSize = new(940, 940);
        Title = title;
        GameManager = gm;
        Shown += (_, _) => Update(true);
        Destroyed += (_, _) => Environment.Exit(0);

        _hPaned.Add1(_lvPaned);
        _hPaned.Add2(_rvPaned);

        _typeListScroll.WidthRequest = 240;
        _typeListScroll.Add(_typeList);

        _lvPaned.Pack1(_typeListScroll, true, false);
        _lvPaned.Pack2(_controlList, false, false);

        _infoListScroll.Add(_infoList);

        _errorListScroll.Add(_errorList);

        _logTabs.HeightRequest = 240;
        _logTabs.AppendPage(_infoListScroll, _infoTabLabel);
        _logTabs.AppendPage(_errorListScroll, _errorTabLabel);

        _rvPaned.Pack1(_area, true, false);
        _rvPaned.Pack2(_logTabs, false, false);

        foreach (Gtk.Widget widget in CreateControls())
            _controlList.Add(widget);

        _area.Drawn += DrawGrid;

        Add(_hPaned);
        SetUpLogging();
    }

    private IEnumerable<Gtk.Widget> CreateControls() {
        Gtk.FileChooserButton fileChooser = new("Open a file", Gtk.FileChooserAction.Open);
        fileChooser.AddFilter(new() { Name = "csv files" });
        fileChooser.AddFilter(new() { Name = "all files" });
        fileChooser.Filters[0].AddPattern("*.csv");
        fileChooser.Filters[1].AddPattern("*");
        fileChooser.SetCurrentFolder(Directory.GetCurrentDirectory());
        fileChooser.SelectionChanged += (_, _) => {
            try {
                Sim.GameManager gm = App.LoadGrid(fileChooser.Filename);
                GameManager = gm;
                Update(true);
            }
            catch (Exception e) {
                Gtk.MessageDialog dialog = new(
                    this,
                    Gtk.DialogFlags.Modal,
                    Gtk.MessageType.Error,
                    Gtk.ButtonsType.Ok,
                    e.Message
                );
                dialog.ButtonPressEvent += (_, _) => dialog.Destroy();
                dialog.Title = $"Failed to load {fileChooser.Filename}";
                dialog.ShowAll();
            }
        };
        yield return fileChooser;

        Gtk.CheckButton autorunSwitch = new() { TooltipText = "Autorun", Label = "Autorun" };
        // Autorun
        yield return autorunSwitch;

        Gtk.SpinButton intervalButton = new(100, 10000, 1) { TooltipText = "Millisecond interval" };
        intervalButton.ValueChanged += (_, _) => _updateInterval = intervalButton.ValueAsInt;
        yield return intervalButton;

        Gtk.Button stepButton = new() { TooltipText = "Step the simulation", Label = "Step" };
        stepButton.Clicked += (_, _) => Update();
        yield return stepButton;

        Gtk.Button exitButton = new() { TooltipText = "Quit the program", Label = "Exit" };
        exitButton.Clicked += (_, _) => Destroy();
        yield return exitButton;

        fileChooser.MarginBottom = intervalButton.MarginBottom = 20;
    }

    private void DrawGrid(object sender, Gtk.DrawnArgs args) {
        Cairo.Context context = args.Cr;
        context.Scale(_area.AllocatedWidth, _area.AllocatedHeight);

        float cellWidth = 1f / GameManager.Grid.Width;
        float cellHeight = 1f / GameManager.Grid.Height;

        for (int y = 0; y < GameManager.Grid.Height; y++)
            for (int x = 0; x < GameManager.Grid.Width; x++) {
                if (GameManager.Grid[x, y].Count > 0) {
                    float h = cellHeight / GameManager.Grid[x, y].Count;
                    for (int i = 0; i < GameManager.Grid[x, y].Count; i++) {
                        var color = GameManager.Grid[x, y][i].Info().Color;
                        context.SetSourceRGB(color.R / 255f, color.G / 255f, color.B / 255f);
                        context.Rectangle(
                            x * cellWidth + 0.0005,
                            (y * cellHeight) + (i * h) + 0.0005,
                            cellWidth - 0.001,
                            h - 0.001
                        );
                        context.Fill();
                    }
                }
                else {
                    if ((x & 1) == (y & 1))
                        context.SetSourceRGB(0.7, 0.7, 0.7);
                    else
                        context.SetSourceRGB(0.9, 0.9, 0.9);
                    context.Rectangle(x * cellWidth, y * cellHeight, cellWidth, cellHeight);
                    context.Fill();
                }
            }
    }

    private void Update(bool windowOnly = false) {
        _infoCount = _errorCount = 0;
        foreach (Gtk.Widget child in _infoList.Children)
            _infoList.Remove(child);
        foreach (Gtk.Widget child in _errorList.Children)
            _errorList.Remove(child);

        if (!windowOnly)
            GameManager.Update();

        foreach (var child in _typeList.Children)
            _typeList.Remove(child);

        var info = GameManager.Grid.SelectMany(x => x).GroupBy(x => x.Info());
        foreach (var g in info) {
            Gtk.Label label = new($"{g.Key.Name} ({g.Count()})") { Halign = Gtk.Align.Start };
            label.ModifyFg(Gtk.StateType.Normal, new Gdk.Color(
                (byte)g.Key.Color.R,
                (byte)g.Key.Color.G,
                (byte)g.Key.Color.B
            ));
            _typeList.Add(label);
        }

        _area.QueueDraw();
        _typeList.ShowAll();
    }

    private void SetUpLogging() {
        Logger.InfoLoggers.Add(msg => {
            Gtk.Label label = new(msg) { Halign = Gtk.Align.Start };
            label.Show();
            _infoList.Add(label);
            _infoTabLabel.Text = $"Info ({++_infoCount})";
        });

        Logger.ErrorLoggers.Add(msg => {
            Gtk.Label label = new(msg) { Halign = Gtk.Align.Start };
            label.ModifyFg(Gtk.StateType.Normal, new Gdk.Color(255, 0, 0));
            label.Show();
            _errorList.Add(label);
            _errorTabLabel.Text = $"Errors ({++_errorCount})";
        });
    }
}