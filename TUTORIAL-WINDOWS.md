# TUTORIAL COMPLETO — Cim Rejuvenator no Windows

Este guia foi feito para você conseguir baixar, compilar e testar o mod **sem precisar desta conversa**.

> **IMPORTANTE:** o mod é experimental. Faça backup do save antes do primeiro teste.

## Configuração inicial recomendada

Para uma cidade extremamente envelhecida:

- Chance de rejuvenescimento: **80%**
- Idade depois de rejuvenescer: **40**
- Restaurar saúde: **Ligado**
- Máximo de rejuvenescimentos por dia: **5.000**

Não comece com 100% + limite enorme. Isso pode transformar dezenas de milhares de aposentados em trabalhadores de uma vez e explodir o desemprego.

---

# CAMINHO RECOMENDADO: COMPILAR SEM UNITY

Este é o método mais simples para teste local. Ele **não exige ativar licença da Unity** e não precisa abrir o Unity Editor.

O projeto usa diretamente as DLLs já instaladas junto com Cities: Skylines II.

---

# PARTE 1 — Preparar o Windows

## 1. Atualizar e localizar Cities: Skylines II

Abra o Steam, atualize **Cities: Skylines II** e abra o jogo pelo menos uma vez.

O caminho mais comum é:

```text
C:\Program Files (x86)\Steam\steamapps\common\Cities Skylines II
```

Mas o jogo pode estar em outra biblioteca, por exemplo:

```text
D:\SteamLibrary\steamapps\common\Cities Skylines II
```

Dentro da pasta do jogo deve existir:

```text
Cities2_Data\Managed\Game.dll
```

Se `Game.dll` existe ali, esse é o caminho certo.

---

# PARTE 2 — Instalar o .NET SDK

Abra PowerShell:

```powershell
dotnet --version
```

Se aparecer uma versão, pode continuar.

Se não reconhecer `dotnet`, rode como Administrador:

```powershell
winget install Microsoft.DotNet.SDK.10
```

Depois feche e abra o terminal novamente.

---

# PARTE 3 — Instalar Git e baixar o projeto

```powershell
winget install Git.Git
```

Depois feche e abra o terminal.

Crie uma pasta:

```powershell
cd C:\
mkdir CS2Mods -ErrorAction SilentlyContinue
cd C:\CS2Mods
```

Clone:

```powershell
git clone https://github.com/decodxr/CimRejuvenator.git
cd CimRejuvenator
```

Ou baixe o ZIP pelo GitHub em **Code > Download ZIP** e extraia.

---

# PARTE 4 — Verificar o ambiente

Na pasta do projeto:

```powershell
.\check-environment.ps1
```

Se o PowerShell bloquear scripts:

```powershell
Set-ExecutionPolicy -Scope Process Bypass
```

Depois:

```powershell
.\check-environment.ps1
```

O script tenta localizar o jogo automaticamente.

Se encontrar, deve aparecer algo como:

```text
[OK] dotnet encontrado
[OK] Jogo encontrado para build sem Unity: D:\SteamLibrary\steamapps\common\Cities Skylines II
Ambiente pronto para pelo menos um modo de compilacao.
```

## Se o jogo NÃO for encontrado

Defina o caminho manualmente. Exemplo:

```powershell
$env:CSII_GAMEPATH="D:\SteamLibrary\steamapps\common\Cities Skylines II"
```

Confirme:

```powershell
Test-Path "$env:CSII_GAMEPATH\Cities2_Data\Managed\Game.dll"
```

Tem que retornar:

```text
True
```

Depois rode:

```powershell
.\check-environment.ps1
```

---

# PARTE 5 — BUILD SEM UNITY

Rode:

```powershell
.\build-no-unity.ps1
```

Esse script força:

```text
ForceNoUnityBuild=true
```

Então mesmo que a Code Modding Toolchain esteja instalada e esteja pedindo ativação da Unity, ela é ignorada nesse build.

Internamente o projeto referencia diretamente arquivos como:

```text
Cities2_Data\Managed\Game.dll
Cities2_Data\Managed\Colossal.Core.dll
Cities2_Data\Managed\Colossal.Logging.dll
Cities2_Data\Managed\Unity.Entities.dll
Cities2_Data\Managed\Unity.Collections.dll
...
```

Se funcionar, você verá:

```text
BUILD SEM UNITY CONCLUIDO!
```

O pacote para copiar estará em:

```text
dist\CimRejuvenator\CimRejuvenator.dll
```

## Se der erro

Salve tudo em arquivo:

```powershell
.\build-no-unity.ps1 *> build-error.txt
```

O `build-error.txt` fica dentro da pasta do projeto.

---

# PARTE 6 — LEVAR O MOD PARA O LINUX

Você não precisa mover o save para o Windows. O save continua no Linux.

Leve apenas esta pasta gerada:

```text
dist\CimRejuvenator
```

Ela deverá conter:

```text
CimRejuvenator.dll
```

Uma forma simples é compactar a pasta em ZIP e enviar para algum lugar acessível pelos dois sistemas, por exemplo GitHub Release, nuvem, pendrive ou rede local.

No Linux/Proton, a pasta de mods costuma estar em um destes caminhos:

```text
~/.local/share/Steam/steamapps/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II/Mods/
```

ou:

```text
~/.steam/steam/steamapps/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II/Mods/
```

Crie:

```text
Mods/CimRejuvenator/
```

e coloque dentro:

```text
CimRejuvenator.dll
```

Resultado esperado:

```text
Mods/
└── CimRejuvenator/
    └── CimRejuvenator.dll
```

Se sua Steam Library estiver em outro disco, procure pela pasta:

```text
steamapps/compatdata/949230/pfx/
```

`949230` é o AppID do Cities: Skylines II.

---

# PARTE 7 — PRIMEIRO TESTE NO JOGO

## 1. Faça backup do save

Não teste na única cópia da cidade.

Use:

```text
SAVE ORIGINAL -> guardar
SAVE TESTE    -> usar com o mod
```

## 2. Abra Cities: Skylines II no Linux

Procure:

```text
Opções > Mods > Cim Rejuvenator
```

Configure:

```text
Ativar mod:                     SIM
Chance de rejuvenescimento:     80%
Idade depois de rejuvenescer:   40
Restaurar saúde:                SIM
Máximo por dia:                 5000
```

Carregue a cidade pausada e comece em velocidade **1x**.

Observe:

- porcentagem de idosos;
- porcentagem de adultos;
- desemprego;
- mortes/mês;
- trabalhadores disponíveis;
- abandono de prédios.

Depois de estabilizar, teste 2x.

---

# PARTE 8 — AJUSTAR OS VALORES

## Recomendado

```text
Chance:       80%
Limite/dia:   5000
Idade:        40
Saúde:        ligada
```

## Se o desemprego explodir

```text
1500–2500 por dia
```

## Se a death wave continuar enorme

Temporariamente:

```text
7500–10000 por dia
```

Depois reduza novamente.

## Quase imortal contra velhice

```text
Chance: 100%
```

Mesmo assim, mantenha limite diário razoável.

Doença e acidente ainda podem matar.

---

# PARTE 9 — ATUALIZAR O MOD

No Windows:

```powershell
cd C:\CS2Mods\CimRejuvenator
git pull
.\build-no-unity.ps1
```

Depois substitua no Linux a pasta antiga `CimRejuvenator` pela nova gerada em `dist`.

---

# PARTE 10 — ERROS COMUNS

## `dotnet` não é reconhecido

```powershell
winget install Microsoft.DotNet.SDK.10
```

Feche e abra o terminal.

## O jogo não foi encontrado

Descubra a pasta do jogo no Steam:

**Biblioteca > Cities: Skylines II > engrenagem > Gerenciar > Procurar arquivos locais**

Depois use, por exemplo:

```powershell
$env:CSII_GAMEPATH="D:\SteamLibrary\steamapps\common\Cities Skylines II"
```

Teste:

```powershell
Test-Path "$env:CSII_GAMEPATH\Cities2_Data\Managed\Game.dll"
```

Deve retornar `True`.

## A toolchain pede ativação da Unity

Não precisa usar a toolchain para o build local deste projeto.

Use:

```powershell
.\build-no-unity.ps1
```

Esse script força o fallback direto pelas DLLs do jogo.

## Erro `MSB3245` / assembly não encontrado

Confirme primeiro:

```powershell
Test-Path "$env:CSII_GAMEPATH\Cities2_Data\Managed\Game.dll"
```

Se for `False`, o caminho está errado.

Se `Game.dll` existe mas alguma outra DLL estiver ausente, salve o log:

```powershell
.\build-no-unity.ps1 *> build-error.txt
```

## Erros `CSxxxx`

Salve:

```powershell
.\build-no-unity.ps1 *> build-error.txt
```

Atualizações do jogo podem mudar nomes de APIs e exigir ajustes no código.

## Build concluiu mas não achei o DLL

Procure em:

```text
dist\CimRejuvenator\CimRejuvenator.dll
```

O script cria essa pasta automaticamente quando a build termina.

## O jogo abre, mas o mod não aparece no Linux

1. Confirme que `CimRejuvenator.dll` está dentro de `Mods/CimRejuvenator/`.
2. Reinicie o jogo.
3. Confirme que code mods estão habilitados.
4. Procure `CimRejuvenator` nos logs do jogo.
5. Confirme que você está usando o prefixo Proton correto da instalação atual do CS2.

## O save ficou estranho

Não sobrescreva o save original.

Saia sem salvar e volte para o backup.

---

# CAMINHO ALTERNATIVO — TOOLCHAIN OFICIAL

Se no futuro você quiser publicar no Paradox Mods, pode usar a toolchain oficial.

Com `CSII_TOOLPATH` configurado:

```powershell
.\build.ps1
```

Esse caminho pode envolver Unity/ativação da licença conforme a instalação da ferramenta oficial.

Para **testar localmente**, use preferencialmente:

```powershell
.\build-no-unity.ps1
```

---

# RESUMO ULTRARRÁPIDO

```powershell
winget install Git.Git
winget install Microsoft.DotNet.SDK.10

git clone https://github.com/decodxr/CimRejuvenator.git
cd CimRejuvenator

Set-ExecutionPolicy -Scope Process Bypass
.\check-environment.ps1
.\build-no-unity.ps1
```

Se o jogo estiver em outro disco:

```powershell
$env:CSII_GAMEPATH="D:\SteamLibrary\steamapps\common\Cities Skylines II"
.\build-no-unity.ps1
```

Depois copie:

```text
dist\CimRejuvenator
```

para a pasta de mods do CS2 no Linux/Proton.

No jogo, comece com:

```text
80% / idade 40 / saúde ligada / 5000 por dia
```
