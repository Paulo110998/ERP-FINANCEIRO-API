
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
public class DownloadContasPagarController : ControllerBase
{
    private readonly EntidadesContext _context;
    private readonly IMapper _mapper;

    public DownloadContasPagarController(EntidadesContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public IActionResult DownloadContasPagar()
    {
        List<ContasPagarRecorrencia> contasPagar = _context.ContasPagarRecorrencia.ToList();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                // Carregar a imagem da logo
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
                                        text.Span("Contas a Pagar");
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
                        // Definindo as colunas da tabela
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1); // Fornecedor
                            columns.RelativeColumn(1); // Descrição
                            columns.RelativeColumn(1); // Recorrência
                            columns.RelativeColumn(1); // Data de Pagamento
                            columns.RelativeColumn(1); // Fim da Recorrência
                            columns.RelativeColumn(1); // Categoria
                            columns.RelativeColumn(1); // Beneficiário
                            columns.RelativeColumn(1); // Valor
                        });

                        // Cabeçalho da tabela
                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Fornecedor");
                            header.Cell().Element(CellStyle).Text("Descrição");
                            header.Cell().Element(CellStyle).Text("Recorrência");
                            header.Cell().Element(CellStyle).Text("Data de Pagamento");
                            header.Cell().Element(CellStyle).Text("Fim da Recorrência");
                            header.Cell().Element(CellStyle).Text("Categoria");
                            header.Cell().Element(CellStyle).Text("Beneficiário");
                            header.Cell().Element(CellStyle).Text("Valor");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container
                                    .Padding(2)
                                    .Background(Colors.Grey.Lighten3)
                                    .DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Black))
                                    .AlignCenter();
                            }
                        });

                        // Dados da tabela
                        foreach (var conta in contasPagar)
                        {
                            table.Cell().Element(CellStyle2).Text(conta.Fornecedor);
                            table.Cell().Element(CellStyle2).Text(conta.DescricaoDespesa);
                            table.Cell().Element(CellStyle2).Text(conta.TipoRecorrencia);
                            table.Cell().Element(CellStyle2).Text(conta.DataInicioRecorrencia?.ToString("dd/MM/yyyy"));
                            table.Cell().Element(CellStyle2).Text(conta.DataVencimento.ToString("dd/MM/yyyy"));
                            table.Cell().Element(CellStyle2).Text(GetCategoriaTitulo(conta.CategoriasId));
                            table.Cell().Element(CellStyle2).Text(GetBeneficiarioNome(conta.BeneficiariosId));
                            table.Cell().Element(CellStyle2).Text($"{conta.Valor} R$");

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
        //Response.Headers.Add("Access-Control-Allow-Origin", "*");
        Response.Headers.Add("Content-Disposition", "attachment; filename=Contas-Pagar.pdf");
        Response.Headers.Add("Content-Type", "application/pdf");

        // Retorna o arquivo PDF diretamente
        return File(document, "application/pdf", "ContasPagar.pdf");
    }

    [HttpHead] // Método HEAD para verificar a existência do recurso
    public IActionResult Head()
    {
        // Retorna apenas um OK para o método HEAD
        return Ok();
    }

    // Método para obter o título da categoria
    private string GetCategoriaTitulo(int? categoriaId)
    {
        if (categoriaId.HasValue)
        {
            var categoria = _context.Categorias.FirstOrDefault(c => c.Id == categoriaId);
            return categoria != null ? categoria.TituloCategoria : "Não adicionado..";
        }
        return "Categoria não encontrada";
    }

    // Método para obter o nome do beneficiário
    private string GetBeneficiarioNome(int? beneficiarioId)
    {
        if (beneficiarioId.HasValue)
        {
            var beneficiario = _context.Beneficiarios.FirstOrDefault(b => b.Id == beneficiarioId);
            return beneficiario != null ? beneficiario.NomeBeneficiario : "Não adicionado..";
        }
        return "Beneficiário não encontrado";
    }
}
