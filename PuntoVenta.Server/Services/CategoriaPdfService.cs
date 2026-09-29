using PuntoVenta.Shared.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PuntoVenta.Server.Services;

public interface ICategoriaPdfService
{
    byte[] GenerarListado(IReadOnlyCollection<Categoria> categorias);
}

public class CategoriaPdfService : ICategoriaPdfService
{
    public byte[] GenerarListado(IReadOnlyCollection<Categoria> categorias)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(36);
                page.Header().Text("Listado de categorías").FontSize(20).SemiBold();
                page.Content().PaddingVertical(16).Column(column =>
                {
                    column.Item().Text($"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(10).FontColor(Colors.Grey.Darken1);
                    column.Item().PaddingTop(12).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(50);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(3);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(EstiloEncabezado).Text("ID");
                            header.Cell().Element(EstiloEncabezado).Text("Nombre");
                            header.Cell().Element(EstiloEncabezado).Text("Descripción");
                        });

                        foreach (var categoria in categorias)
                        {
                            table.Cell().Element(EstiloCelda).Text(categoria.Id.ToString());
                            table.Cell().Element(EstiloCelda).Text(categoria.Nombre);
                            table.Cell().Element(EstiloCelda).Text(categoria.Descripcion ?? "-");
                        }
                    });
                });
                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Total de categorías: ");
                    text.Span(categorias.Count.ToString()).SemiBold();
                });
            });
        }).GeneratePdf();
    }

    private static IContainer EstiloEncabezado(IContainer container)
    {
        return container.Background(Colors.Blue.Darken2).Padding(6).DefaultTextStyle(x => x.FontColor(Colors.White).SemiBold());
    }

    private static IContainer EstiloCelda(IContainer container)
    {
        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(6);
    }
}
