using PuntoVenta.Shared.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PuntoVenta.Server.Services;

public interface IReportePdfService
{
    byte[] GenerarListadoCategorias(IReadOnlyCollection<Categoria> categorias);
    byte[] GenerarListadoProductos(IReadOnlyCollection<Producto> productos);
    byte[] GenerarListadoVentas(IReadOnlyCollection<Venta> ventas);
}

public class ReportePdfService : IReportePdfService
{
    public byte[] GenerarListadoCategorias(IReadOnlyCollection<Categoria> categorias)
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

    public byte[] GenerarListadoProductos(IReadOnlyCollection<Producto> productos)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(36);
                page.Header().Text("Listado de productos").FontSize(20).SemiBold();
                page.Content().PaddingVertical(16).Column(column =>
                {
                    column.Item().Text($"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(10).FontColor(Colors.Grey.Darken1);
                    column.Item().PaddingTop(12).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(1.2f);
                            columns.RelativeColumn(1.2f);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(EstiloEncabezado).Text("ID");
                            header.Cell().Element(EstiloEncabezado).Text("Nombre");
                            header.Cell().Element(EstiloEncabezado).Text("Categoría");
                            header.Cell().Element(EstiloEncabezado).AlignRight().Text("Precio");
                            header.Cell().Element(EstiloEncabezado).AlignRight().Text("Stock");
                            header.Cell().Element(EstiloEncabezado).AlignRight().Text("Stock mín.");
                        });

                        foreach (var producto in productos)
                        {
                            table.Cell().Element(EstiloCelda).Text(producto.Id.ToString());
                            table.Cell().Element(EstiloCelda).Text(producto.Nombre);
                            table.Cell().Element(EstiloCelda).Text(producto.Categoria?.Nombre ?? "-");
                            table.Cell().Element(EstiloCelda).AlignRight().Text(producto.Precio.ToString("C2"));
                            table.Cell().Element(EstiloCelda).AlignRight().Text(producto.StockActual.ToString());
                            table.Cell().Element(EstiloCelda).AlignRight().Text(producto.StockMinimo.ToString());
                        }
                    });
                });
                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Total de productos: ");
                    text.Span(productos.Count.ToString()).SemiBold();
                });
            });
        }).GeneratePdf();
    }

    public byte[] GenerarListadoVentas(IReadOnlyCollection<Venta> ventas)
    {
        var cultura = System.Globalization.CultureInfo.GetCultureInfo("es-AR");
        var totalConfirmadas = ventas.Where(v => v.Estado == PuntoVenta.Shared.Enums.EstadoVenta.Confirmada).Sum(v => v.MontoTotal);

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(36);
                page.Header().Text("Listado de ventas").FontSize(20).SemiBold();
                page.Content().PaddingVertical(16).Column(column =>
                {
                    column.Item().Text($"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(10).FontColor(Colors.Grey.Darken1);
                    column.Item().PaddingTop(12).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(EstiloEncabezado).Text("Nº");
                            header.Cell().Element(EstiloEncabezado).Text("Fecha");
                            header.Cell().Element(EstiloEncabezado).Text("Forma de pago");
                            header.Cell().Element(EstiloEncabezado).Text("Estado");
                            header.Cell().Element(EstiloEncabezado).AlignRight().Text("Total");
                        });

                        foreach (var venta in ventas)
                        {
                            table.Cell().Element(EstiloCelda).Text(venta.Id.ToString());
                            table.Cell().Element(EstiloCelda).Text(venta.Fecha.ToString("dd/MM/yyyy HH:mm"));
                            table.Cell().Element(EstiloCelda).Text(venta.FormaPago.ToString());
                            table.Cell().Element(EstiloCelda).Text(venta.Estado.ToString());
                            table.Cell().Element(EstiloCelda).AlignRight().Text(venta.MontoTotal.ToString("C2", cultura));
                        }
                    });
                });
                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span($"Total de ventas: {ventas.Count} | Total confirmadas: ");
                    text.Span(totalConfirmadas.ToString("C2", cultura)).SemiBold();
                });
            });
        }).GeneratePdf();
    }

    private static IContainer EstiloEncabezado(IContainer container)    {
        return container.Background(Colors.Blue.Darken2).Padding(6).DefaultTextStyle(x => x.FontColor(Colors.White).SemiBold());
    }

    private static IContainer EstiloCelda(IContainer container)
    {
        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(6);
    }
}
