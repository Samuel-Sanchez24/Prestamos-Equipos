using MudBlazor;

namespace Prestamos.Web.Theme;

public static class PrestamosTheme
{
    public static readonly MudTheme Tema = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#4F46E5",
            Secondary = "#64748B",
            Background = "#F8FAFC",
            Surface = "#FFFFFF",
            AppbarBackground = "#FFFFFF",
            AppbarText = "#0F172A",
            DrawerBackground = "#0F172A",
            DrawerText = "#CBD5E1",
            DrawerIcon = "#94A3B8",
            TextPrimary = "#0F172A",
            TextSecondary = "#64748B",
            Success = "#15803D",
            Warning = "#B45309",
            Error = "#B91C1C",
            Info = "#1D4ED8",
            Divider = "#E2E8F0",
            LinesDefault = "#E2E8F0"
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "10px"
        }
    };
}  
