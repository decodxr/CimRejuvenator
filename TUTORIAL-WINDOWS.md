# TUTORIAL COMPLETO — Cim Rejuvenator v0.2.0

Este guia serve para baixar, compilar, levar para o Linux/Proton e testar o mod **sem depender desta conversa**.

> **IMPORTANTE:** faça backup do save antes do primeiro teste. O mod altera diretamente a idade de cidadãos existentes.

---

# 1 — CAMINHO RECOMENDADO: BUILD SEM UNITY

Para teste local, você **não precisa ativar o Unity Editor**.

O projeto usa diretamente as DLLs instaladas do Cities: Skylines II.

## Instalar Git e .NET

Abra PowerShell como administrador:

```powershell
winget install Git.Git
winget install Microsoft.DotNet.SDK.10
```

Feche e abra o terminal depois.

Confira:

```powershell
git --version
dotnet --version
```

---

# 2 — BAIXAR O PROJETO

```powershell
cd C:\Users\$env:USERNAME
git clone https://github.com/decodxr/CimRejuvenator.git
cd CimRejuvenator
```

Se a pasta já existir:

```powershell
cd C:\Users\$env:USERNAME\CimRejuvenator
git pull
```

---

# 3 — LIBERAR SCRIPTS NA JANELA ATUAL

Se o PowerShell disser que scripts estão bloqueados:

```powershell
Set-ExecutionPolicy -Scope Process Bypass
```

Isso vale só para a janela atual.

---

# 4 — COMPILAR SEM UNITY

Primeiro:

```powershell
.\check-environment.ps1
```

Depois:

```powershell
.\build-no-unity.ps1
```

O script tenta localizar o Cities: Skylines II automaticamente.

Se o jogo estiver, por exemplo, em:

```text
D:\SteamLibrary\steamapps\common\Cities Skylines II
```

rode:

```powershell
$env:CSII_GAMEPATH="D:\SteamLibrary\steamapps\common\Cities Skylines II"
.\build-no-unity.ps1
```

O resultado esperado é algo equivalente a:

```text
Build succeeded.
0 Error(s)
```

A DLL pronta deve ficar em:

```text
dist\CimRejuvenator\CimRejuvenator.dll
```

---

# 5 — SE A BUILD DER ERRO

Salve o log inteiro:

```powershell
.\build-no-unity.ps1 *> build-error.txt
```

O arquivo será criado na pasta do projeto:

```text
build-error.txt
```

Erros `CSxxxx` normalmente significam que alguma classe/campo da API do jogo mudou e o código precisa ser atualizado para a versão instalada.

---

# 6 — LEVAR PARA O LINUX / PROTON

Você **não precisa mover o save para o Windows**.

Leve apenas:

```text
dist\CimRejuvenator
```

para o Linux.

O AppID do Cities: Skylines II é `949230`.

O caminho típico é:

```text
~/.local/share/Steam/steamapps/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II/Mods/CimRejuvenator/
```

No final precisa existir:

```text
.../Mods/CimRejuvenator/CimRejuvenator.dll
```

Se a pasta `Mods` não existir:

```bash
mkdir -p "$HOME/.local/share/Steam/steamapps/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II/Mods/CimRejuvenator"
```

Depois copie a DLL para ela.

> Não coloque a DLL dentro de `.cache/Mods/pdx_mods`. Essa área é usada pelo cache do Paradox Mods.

---

# 7 — PRIMEIRO TESTE NO JOGO

Abra o jogo e procure:

```text
Opções > Mods > Cim Rejuvenator
```

Na v0.2.0 você deverá encontrar:

```text
Ativar Cim Rejuvenator
Chance de rejuvenescimento
Idade interna após rejuvenescer
Restaurar saúde mínima
REJUVENESCER AGORA
Máximo por dia
Máximo por varredura
Manter porcentagem mínima de idosos
Porcentagem mínima de idosos
Varreduras automáticas por dia
Estatísticas
```

A primeira varredura automática ocorre assim que a simulação começa.

O padrão agora é:

```text
Chance:                         80%
Idade:                           40
Saúde mínima:               ligada
Máximo por dia:             20.000
Máximo por varredura:        5.000
Varreduras por dia:              64
Proteção mínima de idosos: desligada
```

---

# 8 — TESTE AGRESSIVO PARA VER SE FUNCIONA

Se a cidade está com uma quantidade absurda de idosos e você quer apenas confirmar que o mod está funcionando:

```text
Chance:                    100%
Máximo por dia:         100.000
Máximo por varredura:    50.000
Varreduras por dia:          64
```

Feche a tela de Opções, despause a cidade e deixe rodar.

Ou pressione:

```text
REJUVENESCER AGORA
```

Depois feche as opções e despause.

O botão agenda uma varredura imediata, mas ainda respeita:

- chance;
- limite por dia;
- limite por varredura;
- proteção demográfica.

Se quiser um teste ainda mais extremo, o limite máximo atual é:

```text
250.000 por dia
100.000 por varredura
```

Use isso só por pouco tempo.

---

# 9 — PROTEÇÃO DEMOGRÁFICA

Depois de sair da death wave, é melhor evitar transformar praticamente todo idoso em adulto.

Ative:

```text
Manter porcentagem mínima de idosos: ligado
Porcentagem mínima de idosos:        15%
```

O mod calcula quantos idosos podem ser rejuvenescidos sem cair abaixo desse alvo aproximado.

---

# 10 — COMO LER AS ESTATÍSTICAS

A interface mostra:

```text
Cidadãos na última varredura
Idosos vivos na última varredura
Porcentagem de idosos
Rejuvenescidos na última varredura
Rejuvenescidos hoje
Rejuvenescidos nesta sessão
Varreduras nesta sessão
Último dia de simulação analisado
```

Se tudo continuar em `0` mesmo com a simulação rodando, confira os logs.

No Linux:

```bash
grep -RniE "CimRejuvenator|RejuvenationSystem|Exception|ERROR" \
"$HOME/.local/share/Steam/steamapps/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II/Logs" \
| tail -200
```

O arquivo próprio do mod normalmente é:

```text
.../Cities Skylines II/Logs/CimRejuvenator.log
```

Uma varredura bem-sucedida gera uma linha parecida com:

```text
Completed automatic rejuvenation sweep: scanned=..., seniors=..., rejuvenated=...
```

ou:

```text
Completed manual rejuvenation sweep: ...
```

---

# 11 — ATUALIZAR O MOD

Quando houver código novo, no Windows:

```powershell
cd C:\Users\$env:USERNAME\CimRejuvenator
git pull
Set-ExecutionPolicy -Scope Process Bypass
.\build-no-unity.ps1
```

Depois substitua no Linux:

```text
.../Mods/CimRejuvenator/CimRejuvenator.dll
```

Feche completamente o Cities: Skylines II antes de substituir a DLL e abra novamente depois.

---

# 12 — CONFIGURAÇÃO RECOMENDADA APÓS A CIDADE ESTABILIZAR

Uma configuração mais segura para uso contínuo:

```text
Chance:                         75–85%
Idade:                              40
Saúde mínima:                   ligada
Máximo por dia:             10k–25k
Máximo por varredura:        2k–5k
Varreduras por dia:              64
Manter idosos mínimos:        ligado
Mínimo de idosos:             15–20%
```

A porcentagem ideal depende da cidade; não precisa tentar atingir um número exato.

---

# RESUMO ULTRARRÁPIDO

No Windows:

```powershell
cd C:\Users\$env:USERNAME\CimRejuvenator
git pull
Set-ExecutionPolicy -Scope Process Bypass
$env:CSII_GAMEPATH="D:\SteamLibrary\steamapps\common\Cities Skylines II"
.\build-no-unity.ps1
```

Leve:

```text
dist\CimRejuvenator\CimRejuvenator.dll
```

para o Linux em:

```text
.../Cities Skylines II/Mods/CimRejuvenator/
```

Faça backup do save, abra o jogo e teste.
