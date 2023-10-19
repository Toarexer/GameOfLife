namespace GameOfLifeApp;

internal sealed class ErrorPopup : Gtk.Dialog {
    public ErrorPopup(Gtk.Window parent, string title, string message) : base(title, parent, Gtk.DialogFlags.Modal) {
        Resizable = false;

        Gtk.Label label = new(message) { Selectable = true, LineWrap = true, LineWrapMode = Pango.WrapMode.Word };
        Gtk.ScrolledWindow scrolled = new() { HeightRequest = 150 };
        scrolled.Add(label);

        (Child as Gtk.Box)?.PackStart(scrolled, true, true, 0);
        AddActionWidget(new Gtk.Button { Label = "Ok", WidthRequest = 400 }, Gtk.ResponseType.Ok);

        ShowAll();
    }
}
