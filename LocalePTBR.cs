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
                { m_Setting.GetOptionGroupLocaleID(Setting.kStatsGroup), "Estatísticas" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableMod)), "Ativar Cim Rejuvenator" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableMod)), "Liga ou desliga o rejuvenescimento. Desligar não desfaz cidadãos que já foram rejuvenescidos." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RejuvenationChance)), "Chance de rejuvenescimento" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RejuvenationChance)), "Porcentagem dos idosos que voltam a ser adultos neste ciclo de vida. 100% praticamente elimina mortes por velhice, mas doenças e acidentes ainda podem matar." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetAgeDays)), "Idade interna após rejuvenescer" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetAgeDays)), "Idade em dias de simulação atribuída ao cidadão após voltar a Adulto. 40 é um bom ponto inicial." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RestoreHealth)), "Restaurar saúde mínima ao rejuvenescer" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RestoreHealth)), "Eleva a saúde para pelo menos 80 ao rejuvenescer. Não remove doença ou ferimento existente." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.MaxRejuvenationsPerDay)), "Máximo de rejuvenescimentos por dia" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.MaxRejuvenationsPerDay)), "Freio de segurança para não transformar dezenas de milhares de aposentados em trabalhadores de uma vez." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SeniorsLastScan)), "Idosos vivos na última varredura" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SeniorsLastScan)), "Quantidade de cidadãos idosos vivos encontrada na última varredura do mod." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RejuvenatedToday)), "Rejuvenescidos hoje" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RejuvenatedToday)), "Quantidade rejuvenescida no dia atual da simulação." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RejuvenatedSession)), "Rejuvenescidos nesta sessão" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RejuvenatedSession)), "Total rejuvenescido desde que o jogo foi aberto." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetStatistics)), "Zerar estatísticas" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetStatistics)), "Zera os contadores mostrados nesta página." },
                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ResetStatistics)), "Zerar os contadores desta sessão?" },
            };
        }

        public void Unload() { }
    }
}
