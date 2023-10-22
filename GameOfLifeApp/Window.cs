using Sim = GameOfLifeSim;
using GameOfLifeLogger;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GameOfLifeApp;

internal sealed class GameManagerWindow : Gtk.Window {
    public Sim.GameManager GameManager { get; private set; }

    // Panned containers
    private readonly Gtk.Paned _hPaned = new(Gtk.Orientation.Horizontal);
    private readonly Gtk.Paned _lvPaned = new(Gtk.Orientation.Vertical);
    private readonly Gtk.Paned _rvPaned = new(Gtk.Orientation.Vertical);

    // Left container
    private readonly Gtk.ScrolledWindow _typeListScroll = new();
    private readonly Gtk.ListBox _typeList = new();
    private readonly Gtk.ListBox _controlList = new();

    // Main container
    private readonly Gtk.DrawingArea _area = new();

    // Bottom container
    private readonly Gtk.Notebook _logTabs = new();
    private readonly Gtk.Label _infoTabLabel = new() { Text = "Info (0)" };
    private readonly Gtk.Label _errorTabLabel = new() { Text = "Errors (0)" };
    private readonly Gtk.ScrolledWindow _infoListScroll = new();
    private readonly Gtk.ListBox _infoList = new();
    private readonly Gtk.ScrolledWindow _errorListScroll = new();
    private readonly Gtk.ListBox _errorList = new();

    // Controls
    private readonly Gtk.FileChooserButton _fileChooser = new("Open a file", Gtk.FileChooserAction.Open) {
        TooltipText = "Open a csv file with the correct format to load an initial grid state"
    };

    private readonly Gtk.CheckButton _autorunSwitch = new() { TooltipText = "Autorun", Label = "Autorun" };
    private readonly Gtk.SpinButton _intervalButton = new(100, 10000, 1) { TooltipText = "Millisecond interval" };
    private readonly Gtk.Button _stepButton = new() { TooltipText = "Step the simulation", Label = "Step" };
    private readonly Gtk.Button _exitButton = new() { TooltipText = "Quit the program", Label = "Exit" };

    private int _infoCount, _errorCount;

    public GameManagerWindow(string title, Sim.GameManager gm) : base(title) {
        DefaultSize = new(960, 960);
        GameManager = gm;
        Shown += (_, _) => Update(true);
        Destroyed += (_, _) => Gtk.Application.Quit();

        _hPaned.WidthRequest = 480;
        _hPaned.Add1(_lvPaned);
        _hPaned.Add2(_rvPaned);

        _typeListScroll.Add(_typeList);

        _lvPaned.WidthRequest = 240;
        _lvPaned.Pack1(_typeListScroll, true, false);
        _lvPaned.Pack2(_controlList, false, false);

        _infoListScroll.Add(_infoList);

        _errorListScroll.Add(_errorList);

        _logTabs.HeightRequest = 240;
        _logTabs.AppendPage(_infoListScroll, _infoTabLabel);
        _logTabs.AppendPage(_errorListScroll, _errorTabLabel);

        _rvPaned.WidthRequest = 240;
        _rvPaned.Pack1(_area, true, false);
        _rvPaned.Pack2(_logTabs, false, false);

        _controlList.Add(_fileChooser);
        _controlList.Add(new Gtk.Separator(Gtk.Orientation.Horizontal) { Margin = 5 });
        _controlList.Add(_autorunSwitch);
        _controlList.Add(_intervalButton);
        _controlList.Add(new Gtk.Separator(Gtk.Orientation.Horizontal) { Margin = 5 });
        _controlList.Add(_stepButton);
        _controlList.Add(_exitButton);

        _area.HeightRequest = 240;
        _area.Drawn += DrawGrid;

        Add(_hPaned);

        SetUpControls();
        SetUpLogging();
    }

    private void SetUpControls() {
        _fileChooser.AddFilter(new() { Name = "csv files" });
        _fileChooser.AddFilter(new() { Name = "all files" });
        _fileChooser.Filters[0].AddPattern("*.csv");
        _fileChooser.Filters[1].AddPattern("*");
        _fileChooser.SetCurrentFolder(Directory.GetCurrentDirectory());
        _fileChooser.SelectionChanged += (obj, _) => {
            if (obj is Gtk.FileChooserButton fcb && fcb.Files.Length > 0)
                try {
                    Sim.GameManager gm = App.LoadGrid(fcb.Filename);
                    GameManager = gm;
                    Update(true);
                }
                catch (Exception e) {
                    ErrorPopup popup = new(this, fcb.Filename, e.Message);
                    popup.Run();
                    popup.Destroy();
                    fcb.UnselectAll();
                }
        };

        CancellationTokenSource autorunCancellation = new();
        _autorunSwitch.Toggled += (_, _) => {
            _stepButton.Sensitive = !_autorunSwitch.Active;
            if (_autorunSwitch.Active) {
                autorunCancellation = new();
                _ = Autorun(autorunCancellation.Token);
            }
            else
                autorunCancellation.Cancel();
        };

        _stepButton.Clicked += (_, _) => Update();

        _exitButton.Clicked += (_, _) => Destroy();
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

    private void Update(bool windowOnly = false) {
        _infoCount = _errorCount = 0;
        _infoTabLabel.Text = "Info (0)";
        _errorTabLabel.Text = "Errors (0)";

        DestroyChildren(_infoList);
        DestroyChildren(_errorList);
        DestroyChildren(_typeList);

        if (!windowOnly)
            GameManager.Update();

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

    private async Task Autorun(CancellationToken token) {
        while (true) {
            Update();
            try {
                await Task.Delay(_intervalButton.ValueAsInt, token);
            }
            catch (TaskCanceledException) {
                return;
            }
        }
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

    private void DestroyChildren(Gtk.Container container) {
        foreach (Gtk.Widget child in container.Children) {
            container.Remove(child);
            child.Destroy();
        }
    }
}
