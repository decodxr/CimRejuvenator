using System.Collections.Generic;
using Colossal;

namespace CimRejuvenator
{
    public sealed class LocalePTBR : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocalePTBR(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), "Cim Rejuvenator" },
                { m_Setting.GetOptionTabLocaleID(Setting.kSection), "Principal" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kMainGroup), "Geral" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kRejuvenationGroup), "Rejuvenescimento" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kSafetyGroup), "Segurança" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kPerformanceGroup), "Desempenho" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kStatsGroup), "Estatísticas" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableMod)), "Ativar Cim Rejuvenator" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableMod)), "Liga ou desliga o rejuvenescimento. Desligar não desfaz cidadãos que já foram rejuvenescidos." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RejuvenationChance)), "Chance de rejuvenescimento" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RejuvenationChance)), "Porcentagem dos idosos elegíveis que voltam a ser adultos. O sorteio é estável durante o mesmo ciclo de velhice." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetAgeDays)), "Idade interna após rejuvenescer" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetAgeDays)), "Idade em dias de simulação atribuída ao cidadão após voltar a Adulto. 40 é um bom ponto inicial." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RestoreHealth)), "Restaurar saúde mínima ao rejuvenescer" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RestoreHealth)), "Eleva a saúde para pelo menos 80 ao rejuvenescer. Não remove doença ou ferimento existente." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RejuvenateNow)), "REJUVENESCER AGORA" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RejuvenateNow)), "Solicita uma varredura imediata. A execução ocorre assim que a simulação estiver rodando e respeita chance, limite diário, limite por varredura e proteção demográfica." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.MaxRejuvenationsPerDay)), "Máximo de rejuvenescimentos por dia" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.MaxRejuvenationsPerDay)), "Limite total por dia de simulação. Pode chegar a 250.000; valores muito altos podem alterar a economia da cidade rapidamente." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.MaxRejuvenationsPerSweep)), "Máximo por varredura" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.MaxRejuvenationsPerSweep)), "Limita quantos idosos podem ser rejuvenescidos de uma só vez. Ajuda a evitar uma mudança instantânea de dezenas de milhares de cidadãos." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.KeepMinimumSeniorShare)), "Manter porcentagem mínima de idosos" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.KeepMinimumSeniorShare)), "Quando ativado, o mod para de rejuvenescer antes de reduzir os idosos abaixo da porcentagem escolhida." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.MinimumSeniorPercent)), "Porcentagem mínima de idosos" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.MinimumSeniorPercent)), "Proteção demográfica opcional. 15% é um valor equilibrado para uma cidade normal." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SweepsPerDay)), "Varreduras automáticas por dia" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SweepsPerDay)), "Frequência das verificações completas da população. 64 é o padrão. Valores maiores respondem mais rápido, mas fazem mais trabalho de CPU." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CitizensLastScan)), "Cidadãos na última varredura" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CitizensLastScan)), "Quantidade de entidades de cidadãos analisadas pela última varredura." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SeniorsLastScan)), "Idosos vivos na última varredura" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SeniorsLastScan)), "Quantidade de cidadãos idosos vivos encontrada na última varredura do mod." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ElderlyPercentLastScan)), "Idosos na última varredura" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ElderlyPercentLastScan)), "Porcentagem aproximada de idosos encontrada na última varredura." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RejuvenatedLastSweep)), "Rejuvenescidos na última varredura" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RejuvenatedLastSweep)), "Quantidade efetivamente rejuvenescida na varredura mais recente." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RejuvenatedToday)), "Rejuvenescidos hoje" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RejuvenatedToday)), "Quantidade rejuvenescida no dia atual da simulação." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RejuvenatedSession)), "Rejuvenescidos nesta sessão" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RejuvenatedSession)), "Total rejuvenescido desde que o jogo foi aberto." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SweepsSession)), "Varreduras nesta sessão" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SweepsSession)), "Número de varreduras completas executadas nesta sessão." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LastSimulationDay)), "Último dia de simulação analisado" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LastSimulationDay)), "Dia interno da simulação em que ocorreu a última varredura." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetStatistics)), "Zerar estatísticas" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetStatistics)), "Zera os contadores mostrados nesta página." },
                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ResetStatistics)), "Zerar os contadores desta sessão?" },
            };
        }

        public void Unload() { }
    }
}
