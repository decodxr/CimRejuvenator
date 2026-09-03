using System.Collections.Generic;
using Colossal;

namespace CimRejuvenator
{
    /// <summary>
    /// Portuguese localization. English entries are used as a complete fallback and the
    /// user-facing groups, controls, actions, and diagnostics are overridden in Portuguese.
    /// </summary>
    public sealed class LocalePTBR : IDictionarySource
    {
        private readonly CimRejuvenatorSetting m_Setting;

        public LocalePTBR(CimRejuvenatorSetting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            var entries = new Dictionary<string, string>();

            foreach (var pair in new LocaleEN(m_Setting).ReadEntries(errors, indexCounts))
            {
                entries[pair.Key] = pair.Value;
            }

            foreach (var pair in new LocaleDirectEN(m_Setting).ReadEntries(errors, indexCounts))
            {
                entries[pair.Key] = pair.Value;
            }

            entries[m_Setting.GetSettingsLocaleID()] = "Cim Rejuvenator";
            entries[m_Setting.GetOptionTabLocaleID(CimRejuvenatorSetting.kSection)] = "Principal";
            entries[m_Setting.GetOptionGroupLocaleID(CimRejuvenatorSetting.kMainGroup)] = "Geral";
            entries[m_Setting.GetOptionGroupLocaleID(CimRejuvenatorSetting.kRejuvenationGroup)] = "Rejuvenescimento";
            entries[m_Setting.GetOptionGroupLocaleID(CimRejuvenatorSetting.kDemographicsGroup)] = "Demografia";
            entries[m_Setting.GetOptionGroupLocaleID(CimRejuvenatorSetting.kTrendGroup)] = "Tendência da população";
            entries[m_Setting.GetOptionGroupLocaleID(CimRejuvenatorSetting.kImmigrationGroup)] = "Imigração";
            entries[m_Setting.GetOptionGroupLocaleID(CimRejuvenatorSetting.kBirthGroup)] = "Nascimentos";
            entries[m_Setting.GetOptionGroupLocaleID(CimRejuvenatorSetting.kPerformanceGroup)] = "Desempenho";
            entries[m_Setting.GetOptionGroupLocaleID(CimRejuvenatorSetting.kStatsGroup)] = "Estatísticas";

            Label(nameof(CimRejuvenatorSetting.EnableMod), "Ativar Cim Rejuvenator", entries);
            Desc(nameof(CimRejuvenatorSetting.EnableMod), "Chave principal para todos os controles de população.", entries);
            Label(nameof(CimRejuvenatorSetting.BuildVersion), "Versão carregada", entries);

            Label(nameof(CimRejuvenatorSetting.EnableRejuvenation), "Ativar rejuvenescimento", entries);
            Label(nameof(CimRejuvenatorSetting.RejuvenationChance), "Chance de rejuvenescimento", entries);
            Label(nameof(CimRejuvenatorSetting.ResetAgeDays), "Idade após rejuvenescer", entries);
            Label(nameof(CimRejuvenatorSetting.RestoreHealth), "Restaurar saúde mínima", entries);
            Label(nameof(CimRejuvenatorSetting.MaxRejuvenationsPerDay), "Máximo de rejuvenescimentos por dia", entries);
            Label(nameof(CimRejuvenatorSetting.MaxRejuvenationsPerSweep), "Máximo por varredura", entries);
            Label(nameof(CimRejuvenatorSetting.KeepMinimumSeniorShare), "Manter porcentagem mínima de idosos", entries);
            Label(nameof(CimRejuvenatorSetting.MinimumSeniorPercent), "Porcentagem mínima de idosos", entries);
            Label(nameof(CimRejuvenatorSetting.RejuvenateNow), "REJUVENESCER AGORA", entries);

            Label(nameof(CimRejuvenatorSetting.EnableDemographicBalancer), "Ativar balanceador demográfico", entries);
            Desc(nameof(CimRejuvenatorSetting.EnableDemographicBalancer), "Move gradualmente a população para a distribuição de faixas etárias escolhida.", entries);
            Label(nameof(CimRejuvenatorSetting.TargetChildPercent), "Meta de crianças", entries);
            Label(nameof(CimRejuvenatorSetting.TargetTeenPercent), "Meta de adolescentes", entries);
            Label(nameof(CimRejuvenatorSetting.TargetAdultPercent), "Meta de adultos", entries);
            Label(nameof(CimRejuvenatorSetting.TargetSeniorPercent), "Meta de idosos", entries);
            Label(nameof(CimRejuvenatorSetting.MaxAgeConversionsPerSweep), "Máximo de conversões de idade por varredura", entries);
            Label(nameof(CimRejuvenatorSetting.ProtectWorkersWhenBalancing), "Proteger adultos empregados", entries);
            Label(nameof(CimRejuvenatorSetting.BalanceNow), "BALANCEAR AGORA", entries);
            Label(nameof(CimRejuvenatorSetting.TargetWeightTotal), "Total dos pesos demográficos", entries);

            Label(nameof(CimRejuvenatorSetting.EnablePopulationTrendControl), "Ativar controle de tendência da população", entries);
            Desc(nameof(CimRejuvenatorSetting.EnablePopulationTrendControl), "Controla o crescimento ou queda líquida da população residente.", entries);
            Label(nameof(CimRejuvenatorSetting.TargetNetPopulationChangePerDay), "Meta líquida de população por dia", entries);
            Desc(nameof(CimRejuvenatorSetting.TargetNetPopulationChangePerDay), "Valor positivo força crescimento; zero tenta impedir qualquer queda; valor negativo permite redução.", entries);
            Label(nameof(CimRejuvenatorSetting.DirectTrendMode), "Trava contínua de crescimento", entries);
            Desc(nameof(CimRejuvenatorSetting.DirectTrendMode), "Com meta zero ou positiva, protege um piso populacional que nunca diminui e agenda novas famílias sempre que a cidade ficar abaixo dele. Reage várias vezes por dia.", entries);
            Label(nameof(CimRejuvenatorSetting.DirectTrendCorrectionStrength), "Força da correção direta", entries);
            Desc(nameof(CimRejuvenatorSetting.DirectTrendCorrectionStrength), "Porcentagem do déficit corrigida em cada verificação. Use 100% durante ondas graves de mortes.", entries);
            Label(nameof(CimRejuvenatorSetting.DirectTrendMaxInjectedResidentsPerDay), "Máximo de moradores diretos por dia", entries);
            Label(nameof(CimRejuvenatorSetting.DirectTrendMaxInjectedResidentsPerCheck), "Máximo de moradores diretos por verificação", entries);
            Desc(nameof(CimRejuvenatorSetting.DirectTrendMaxInjectedResidentsPerCheck), "Limita cada rajada de correção. Valores maiores reagem mais rápido a quedas extremas.", entries);
            Label(nameof(CimRejuvenatorSetting.TrendResponseStrength), "Força de resposta adaptativa", entries);
            Label(nameof(CimRejuvenatorSetting.TrendDeadband), "Margem de tolerância da tendência", entries);
            Label(nameof(CimRejuvenatorSetting.TrendUseImmigration), "Usar imigração no controle", entries);
            Label(nameof(CimRejuvenatorSetting.TrendUseBirths), "Usar nascimentos no controle", entries);
            Label(nameof(CimRejuvenatorSetting.TrendMaximumBirthRatePercent), "Taxa máxima automática de nascimentos", entries);
            Label(nameof(CimRejuvenatorSetting.TrendAllowForcedOutflow), "Permitir saída forçada para metas negativas", entries);
            Label(nameof(CimRejuvenatorSetting.TrendMaxForcedOutflowPerDay), "Máximo de saída forçada por dia", entries);
            Label(nameof(CimRejuvenatorSetting.EmergencyGrowthPreset), "APLICAR MODO DE CRESCIMENTO DE EMERGÊNCIA", entries);
            Desc(nameof(CimRejuvenatorSetting.EmergencyGrowthPreset), "Ativa a trava direta com meta de +5.000/dia, correção de 100%, imigração e natalidade reforçadas e sem expulsão de moradores.", entries);
            Label(nameof(CimRejuvenatorSetting.ResetTrendController), "REINICIAR CONTROLADOR DE TENDÊNCIA", entries);

            Label(nameof(CimRejuvenatorSetting.EnableImmigrationControl), "Ativar controle manual de imigração", entries);
            Label(nameof(CimRejuvenatorSetting.ImmigrationIntensity), "Intensidade da imigração", entries);
            Label(nameof(CimRejuvenatorSetting.UseImmigrationDailyCap), "Usar limite diário de novos moradores", entries);
            Label(nameof(CimRejuvenatorSetting.MaxNewResidentsPerDay), "Máximo de novos moradores por dia", entries);
            Label(nameof(CimRejuvenatorSetting.UsePopulationCeiling), "Usar teto de população", entries);
            Label(nameof(CimRejuvenatorSetting.PopulationCeiling), "Teto de população residente", entries);
            Label(nameof(CimRejuvenatorSetting.ShapeNewResidentAges), "Controlar idades de quem chega", entries);
            Label(nameof(CimRejuvenatorSetting.IncomingChildWeight), "Peso de crianças chegando", entries);
            Label(nameof(CimRejuvenatorSetting.IncomingTeenWeight), "Peso de adolescentes chegando", entries);
            Label(nameof(CimRejuvenatorSetting.IncomingAdultWeight), "Peso de adultos chegando", entries);
            Label(nameof(CimRejuvenatorSetting.IncomingSeniorWeight), "Peso de idosos chegando", entries);
            Label(nameof(CimRejuvenatorSetting.IncomingWeightTotal), "Total dos pesos de quem chega", entries);

            Label(nameof(CimRejuvenatorSetting.EnableBirthControl), "Ativar controle manual de nascimentos", entries);
            Label(nameof(CimRejuvenatorSetting.BirthRatePercent), "Multiplicador da taxa de nascimentos", entries);
            Label(nameof(CimRejuvenatorSetting.UseBirthDailyCap), "Usar limite diário de nascimentos", entries);
            Label(nameof(CimRejuvenatorSetting.MaxBirthsPerDay), "Máximo de nascimentos por dia", entries);
            Label(nameof(CimRejuvenatorSetting.BirthsRespectChildTarget), "Pausar nascimentos ao atingir meta de crianças", entries);

            Label(nameof(CimRejuvenatorSetting.SweepsPerDay), "Varreduras de população por dia", entries);

            Label(nameof(CimRejuvenatorSetting.ResidentCount), "Moradores estabelecidos", entries);
            Label(nameof(CimRejuvenatorSetting.ChildCount), "Crianças", entries);
            Label(nameof(CimRejuvenatorSetting.TeenCount), "Adolescentes", entries);
            Label(nameof(CimRejuvenatorSetting.AdultCount), "Adultos", entries);
            Label(nameof(CimRejuvenatorSetting.SeniorCount), "Idosos", entries);
            Label(nameof(CimRejuvenatorSetting.RejuvenatedLastSweep), "Rejuvenescidos na última varredura", entries);
            Label(nameof(CimRejuvenatorSetting.RejuvenatedToday), "Rejuvenescidos hoje", entries);
            Label(nameof(CimRejuvenatorSetting.RejuvenatedSession), "Rejuvenescidos nesta sessão", entries);
            Label(nameof(CimRejuvenatorSetting.AgeConvertedLastSweep), "Conversões demográficas na última varredura", entries);
            Label(nameof(CimRejuvenatorSetting.AgeConvertedSession), "Conversões demográficas nesta sessão", entries);
            Label(nameof(CimRejuvenatorSetting.BirthsToday), "Nascimentos detectados hoje", entries);
            Label(nameof(CimRejuvenatorSetting.BirthsSession), "Nascimentos detectados nesta sessão", entries);
            Label(nameof(CimRejuvenatorSetting.NewResidentsToday), "Novos moradores detectados hoje", entries);
            Label(nameof(CimRejuvenatorSetting.NewResidentsSession), "Novos moradores nesta sessão", entries);
            Label(nameof(CimRejuvenatorSetting.AppliedBirthRate), "Taxa efetiva de nascimentos", entries);
            Label(nameof(CimRejuvenatorSetting.ImmigrationStatus), "Controlador de imigração", entries);
            Label(nameof(CimRejuvenatorSetting.TrendMode), "Modo da tendência populacional", entries);
            Label(nameof(CimRejuvenatorSetting.TrendTarget), "Meta de tendência", entries);
            Label(nameof(CimRejuvenatorSetting.TrendActualLastDay), "Mudança da população no último dia", entries);
            Label(nameof(CimRejuvenatorSetting.TrendSmoothed), "Tendência suavizada", entries);
            Label(nameof(CimRejuvenatorSetting.TrendEffectiveImmigration), "Imigração efetiva da tendência", entries);
            Label(nameof(CimRejuvenatorSetting.TrendEffectiveBirthRate), "Natalidade efetiva da tendência", entries);
            Label(nameof(CimRejuvenatorSetting.TrendGrowthFloor), "Piso de população protegido", entries);
            Desc(nameof(CimRejuvenatorSetting.TrendGrowthFloor), "Piso atual protegido pela trava de crescimento. Ele não diminui enquanto a trava direta estiver ativa.", entries);
            Label(nameof(CimRejuvenatorSetting.TrendShortfallLastCheck), "Déficit na última verificação", entries);
            Label(nameof(CimRejuvenatorSetting.TrendPendingDirectResidents), "Moradores diretos pendentes", entries);
            Label(nameof(CimRejuvenatorSetting.TrendDirectCorrectionRequested), "Última correção direta solicitada", entries);
            Label(nameof(CimRejuvenatorSetting.TrendDirectResidentsToday), "Moradores diretos agendados hoje", entries);
            Label(nameof(CimRejuvenatorSetting.TrendDirectResidentsSession), "Moradores diretos agendados nesta sessão", entries);
            Label(nameof(CimRejuvenatorSetting.TrendDirectHouseholdsToday), "Famílias diretas agendadas hoje", entries);
            Label(nameof(CimRejuvenatorSetting.TrendForcedOutflowToday), "Saída forçada hoje", entries);
            Label(nameof(CimRejuvenatorSetting.TrendForcedOutflowSession), "Saída forçada nesta sessão", entries);
            Label(nameof(CimRejuvenatorSetting.TrendStatus), "Estado do controlador de população", entries);
            Label(nameof(CimRejuvenatorSetting.SweepsSession), "Varreduras nesta sessão", entries);
            Label(nameof(CimRejuvenatorSetting.LastSimulationDay), "Último dia de simulação verificado", entries);
            Label(nameof(CimRejuvenatorSetting.ResetStatistics), "Zerar estatísticas", entries);
            entries[m_Setting.GetOptionWarningLocaleID(nameof(CimRejuvenatorSetting.ResetStatistics))] = "Zerar as estatísticas desta sessão do Cim Rejuvenator?";

            return entries;
        }

        private void Label(string property, string text, Dictionary<string, string> entries)
        {
            entries[m_Setting.GetOptionLabelLocaleID(property)] = text;
        }

        private void Desc(string property, string text, Dictionary<string, string> entries)
        {
            entries[m_Setting.GetOptionDescLocaleID(property)] = text;
        }

        public void Unload() { }
    }
}
