

using DashboardTrilhaEsporte.Enums;
using DashboardTrilhaEsporte.Domain.Entities;
using Microsoft.VisualBasic;


namespace DashboardTrilhaEsporte.Domain.DTOs
{
        public class SkuMarketplaceDadosDTO
        {
                public List<SkuMarketplaceDTO> skuMarketplaceDTOs { get; set; }

                public Dictionary<Eventos, double>? quantidadeTotalRegistrosPorEvento { get; set; }
                public Dictionary<Erros, double>? quantidadeErrosPorTipo { get; set; }

                public int quantidadeTotalRegistro { get; set; }

                public int quantidadeTotalErros { get; set; }

                public decimal somatorioValorFinal { get; set; }

                public DateTime? dataComissaoInicial { get; set; }

                public DateTime? dataComissaoFinal { get; set; }

                public DateTime? dataCicloInicial { get; set; }

                public DateTime? dataCicloFinal { get; set; }




                public SkuMarketplaceDadosDTO(List<SkuMarketplace> skuMarketplaces)
                {
                        this.skuMarketplaceDTOs = SkuMarketplaceDTO.MapearDTOs(skuMarketplaces);

                        this.quantidadeTotalRegistro = this.skuMarketplaceDTOs.Count;

                        this.SomarValorFinal();
                        this.ContarErros();
                        this.ObterIntervalosDatas();
                        this.ContarTipoEvento();
                }


                public SkuMarketplaceDadosDTO(List<SkuMarketplaceDTO> skuMarketplacesDto)
                {
                        this.skuMarketplaceDTOs = skuMarketplacesDto;
                        this.quantidadeTotalRegistro = this.skuMarketplaceDTOs.Count;
                        this.SomarValorFinal();
                        this.ContarErros();
                        this.ContarTipoEvento();

                        this.ObterIntervalosDatas();
                }

                private void SomarValorFinal()
                {
                        this.somatorioValorFinal = this.skuMarketplaceDTOs
                            .Sum(v => v.skuMarketplace.valorFinal);
                }

                private void ContarErros()
                {
                        if (skuMarketplaceDTOs == null)
                                return;

                        if (quantidadeErrosPorTipo == null)
                                quantidadeErrosPorTipo = new Dictionary<Erros, double>();

                        Erros[] errosParaContar = new[]
                        {
                        Erros.ErroComissao,
                        Erros.ValorFinalNegativo,
                        Erros.FaltaDeComisao,
                        Erros.FaltaDataComissao,
                        Erros.ErroDevolucao
                };

                        foreach (var erro in errosParaContar)
                        {
                                int quantidade = skuMarketplaceDTOs.Count(v =>
                                v?.listaErros != null && v.listaErros.Contains(erro));

                                quantidadeErrosPorTipo[erro] = quantidade;
                        }

                        quantidadeTotalErros = (int)quantidadeErrosPorTipo.Values.Sum();
                }


                public void ObterIntervalosDatas()
                {
                        var comissoes = this.skuMarketplaceDTOs
                            .Where(x => x.skuMarketplace.dataComissao.HasValue)
                            .Select(x => x.skuMarketplace.dataComissao.Value)
                            .ToList();

                        var ciclos = this.skuMarketplaceDTOs
                            .Where(x => x.skuMarketplace.dataCiclo.HasValue)
                            .Select(x => x.skuMarketplace.dataCiclo.Value)
                            .ToList();

                        this.dataComissaoInicial = comissoes.Any() ? comissoes.Min() : null;
                        this.dataComissaoFinal = comissoes.Any() ? comissoes.Max() : null;
                        this.dataCicloInicial = ciclos.Any() ? ciclos.Min() : null;
                        this.dataCicloFinal = ciclos.Any() ? ciclos.Max() : null;
                }


                public void ContarTipoEvento()
                {
                        this.quantidadeTotalRegistrosPorEvento = new Dictionary<Eventos, double>();



                        foreach (var item in this.skuMarketplaceDTOs)
                        {
                                Eventos tipo = item.skuMarketplace.tipoEventoNormalizado;

                                if (!quantidadeTotalRegistrosPorEvento.ContainsKey(tipo))
                                        quantidadeTotalRegistrosPorEvento[tipo] = 0;

                                this.quantidadeTotalRegistrosPorEvento[tipo]++;

                        }
                }

                public Dictionary<string, double> ObterErros()
                {
                        Erros[] erros = new[]
                        {
                                Erros.ErroComissao,
                                Erros.ValorFinalNegativo,
                                Erros.FaltaDeComisao,
                                Erros.FaltaDataComissao,
                                Erros.ErroDevolucao
                        };


                        var resultado = new Dictionary<string, double>();

                        foreach (var erro in erros)
                        {
                                if (quantidadeErrosPorTipo != null && quantidadeErrosPorTipo.TryGetValue(erro, out var count))
                                {
                                        resultado.Add(erro.GetDescription(), count);
                                }
                                else
                                {
                                        resultado.Add(erro.GetDescription(), 0);
                                }
                        }

                        return resultado;
                }

                public Dictionary<string, double> ObterDistribuicaoEventosEmPorcentagem()
                {
                        var resultado = new Dictionary<string, double>();

                        if (quantidadeTotalRegistrosPorEvento == null || !quantidadeTotalRegistrosPorEvento.Any()){
                                Console.WriteLine();
                                return resultado;
                        }
                               

                        double total = this.quantidadeTotalRegistro;
                        if (total == 0)
                                return resultado;

                        foreach (var kvp in quantidadeTotalRegistrosPorEvento)
                        {
                                string chave = kvp.Key.GetDescription();
                                double porcentagem = (kvp.Value);
                                resultado[chave] = porcentagem;
                        }

                        return resultado;
                }





        }
}
