using ERP_Financeiro_API.Data;
using ERP_Financeiro_API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ERP_Financeiro_API.Controllers;

[ApiController]
[Route("[controller]")]
public class DownloadBeneficiariosController : ControllerBase
{
    private readonly EntidadesContext _context;
    private readonly IMapper _mapper;

    public DownloadBeneficiariosController(EntidadesContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public IActionResult DownloadBeneficiarios()
    {
        List<Beneficiarios> beneficiarios = _context.Beneficiarios.ToList();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                // Carregar a imagem da logo (mesmo código do primeiro controlador)
                var logoPath = Path.Combine("Img-pdf", "logo.png");
                byte[] logoData = System.IO.File.ReadAllBytes(logoPath);

                page.Header()
                    .Padding(13)
                    .Row(row =>
                    {
                        row.RelativeColumn()
                            .Column(column =>
                            {
                                column.Item().Height(25); // Espaço acima do título para a imagem
                                column.Item()
                                    .Text(text =>
                                    {
                                        text.DefaultTextStyle(x => x.FontSize(25).FontColor(Colors.Blue.Darken4));
                                        text.AlignLeft();
                                        text.Span("Beneficiários");
                                    });
                            });
                        row.ConstantColumn(80)
                            .AlignRight()
                            .AlignMiddle()
                            .Image(logoData);
                    });

                page.Content()
                    .Padding(15)
                    .Table(table =>
                    {
                        // Definindo as colunas da tabela (mesmo código do primeiro controlador)
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1); // Beneficiário
                            columns.RelativeColumn(1); // CPF/CNPJ
                            columns.RelativeColumn(1); // Referência
                            columns.RelativeColumn(1); // Tipo
                            columns.RelativeColumn(1); // Descrição
                        });

                        // Cabeçalho da tabela (mesmo código do primeiro controlador)
                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Beneficiário");
                            header.Cell().Element(CellStyle).Text("CPF/CNPJ");
                            header.Cell().Element(CellStyle).Text("Referência");
                            header.Cell().Element(CellStyle).Text("Tipo");
                            header.Cell().Element(CellStyle).Text("Descrição");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container
                                    .Padding(2)
                                    .Background(Colors.Grey.Lighten3)
                                    .DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Black))
                                    .AlignCenter();
                            }
                        });

                        // Dados da tabela (mesmo código do primeiro controlador)
                        foreach (var item in beneficiarios)
                        {
                            table.Cell().Element(CellStyle2).Text(item.NomeBeneficiario);
                            table.Cell().Element(CellStyle2).Text(item.Cpf_Cnpj);
                            table.Cell().Element(CellStyle2).Text(item.Referencia);
                            table.Cell().Element(CellStyle2).Text(item.Tipo);
                            table.Cell().Element(CellStyle2).Text(item.Descricao);

                            static IContainer CellStyle2(IContainer container2)
                            {
                                return container2
                                    .Border(1)
                                    .BorderColor(Colors.Black)
                                    .DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Black))
                                    .AlignCenter();
                            }
                        }
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
            });
        }).GeneratePdf();

        // Configurando cabeçalhos da resposta HTTP
        Response.Headers.Add("Content-Disposition", "attachment; filename=Beneficiarios.pdf");
        Response.Headers.Add("Content-Type", "application/pdf");


        // Retorna o arquivo PDF diretamente
        return File(document, "application/pdf", "Beneficiarios.pdf");
    }
}
