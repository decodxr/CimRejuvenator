# Cim Rejuvenator v0.2.0

> **Status: experimental. Faça backup do save antes de usar.** O mod altera diretamente a idade dos cidadãos de Cities: Skylines II.

O **Cim Rejuvenator** transforma uma porcentagem configurável dos cidadãos idosos em adultos novamente, preservando o mesmo cidadão.

## 🚀 Build sem Unity

Para teste local, o projeto pode ser compilado **sem abrir/ativar o Unity Editor**, usando diretamente as DLLs instaladas do jogo:

```powershell
git clone https://github.com/decodxr/CimRejuvenator.git
cd CimRejuvenator
Set-ExecutionPolicy -Scope Process Bypass
.\check-environment.ps1
.\build-no-unity.ps1
```

Se o jogo estiver em outro caminho:

```powershell
$env:CSII_GAMEPATH="D:\SteamLibrary\steamapps\common\Cities Skylines II"
.\build-no-unity.ps1
```

A saída fica em:

```text
dist\CimRejuvenator\CimRejuvenator.dll
```

Para o tutorial completo, veja **[TUTORIAL-WINDOWS.md](TUTORIAL-WINDOWS.md)**.

## ✨ Novidades da v0.2.0

- **64 varreduras automáticas por dia** por padrão, configuráveis de 8 a 256.
- A **primeira varredura acontece assim que a simulação começa**, sem esperar um intervalo inteiro.
- Botão **REJUVENESCER AGORA** para solicitar uma varredura imediata.
- Limite diário aumentado para até **250.000** rejuvenescimentos.
- Novo limite por varredura de até **100.000** para evitar uma mudança gigantesca em um único tick.
- Proteção demográfica opcional: manter pelo menos uma porcentagem escolhida de idosos.
- Mais estatísticas: cidadãos analisados, idosos, porcentagem de idosos, rejuvenescidos na última varredura, total do dia, total da sessão e número de varreduras.
- Logs mais detalhados para diagnosticar se o sistema está realmente executando.
- Continua suportando o modo de build **sem Unity**.

## Como funciona

- Interface em **Opções > Mods > Cim Rejuvenator**.
- Chance de rejuvenescimento de **0% a 100%**.
- O mesmo cidadão volta de `Elderly` para `Adult`; casa, família, educação e identidade continuam pertencendo à mesma entidade.
- A data de nascimento interna é reajustada para corresponder à idade escolhida.
- Pode restaurar a saúde mínima para 80.
- Cidadãos já mortos não são ressuscitados.
- Doença e acidente continuam podendo matar.
- O sorteio da porcentagem é estável durante o mesmo ciclo de velhice; um idoso que falhou em 80% não fica ganhando uma nova tentativa a cada frame.

## Configuração recomendada

Para uma cidade extremamente envelhecida, comece com:

```text
Chance de rejuvenescimento:        80%
Idade depois de rejuvenescer:      40
Restaurar saúde:                   ligado
Máximo por dia:                    20.000
Máximo por varredura:               5.000
Varreduras automáticas por dia:        64
Proteção de mínimo de idosos:      desligada durante a recuperação
```

Se quiser fazer um teste agressivo para confirmar que o mod está funcionando:

```text
Chance:                    100%
Máximo por dia:         100.000
Máximo por varredura:    50.000 ou 100.000
```

Depois volte para valores menores para não transformar a economia da cidade inteira de uma vez.

### Proteção demográfica

Você pode ativar:

```text
Manter porcentagem mínima de idosos: ligado
Porcentagem mínima de idosos:        15%
```

Assim o mod deixa de rejuvenescer quando a cidade se aproxima do percentual configurado.

## Botão REJUVENESCER AGORA

O botão agenda uma varredura para o próximo momento em que a **simulação estiver rodando**. Ele ainda respeita:

- chance de rejuvenescimento;
- máximo diário;
- máximo por varredura;
- proteção de porcentagem mínima de idosos.

Se o jogo estiver pausado ou a tela de Opções tiver pausado a simulação, feche as opções e despause depois de apertar o botão.

## Windows → Linux / Proton

Depois do build, copie:

```text
dist\CimRejuvenator
```

para:

```text
~/.local/share/Steam/steamapps/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II/Mods/CimRejuvenator/
```

No final deve existir:

```text
.../Mods/CimRejuvenator/CimRejuvenator.dll
```

## Atualizar

No Windows:

```powershell
cd C:\Users\SEU_USUARIO\CimRejuvenator
git pull
.\build-no-unity.ps1
```

Depois substitua a DLL antiga no Linux pela nova e reinicie o jogo.

## Logs no Linux

Para confirmar que o mod carregou:

```bash
grep -Rni "CimRejuvenator" \
"~/.local/share/Steam/steamapps/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II/Logs"
```

O log próprio do mod fica normalmente em:

```text
.../Cities Skylines II/Logs/CimRejuvenator.log
```

## Licença

MIT — veja [LICENSE](LICENSE).
