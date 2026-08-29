# TUTORIAL COMPLETO — Cim Rejuvenator no Windows

Este guia foi feito para você conseguir instalar, compilar e testar o mod **sem precisar desta conversa**.

> **IMPORTANTE:** o mod é experimental. Faça backup do save antes do primeiro teste.

## Configuração inicial recomendada

Para uma cidade extremamente envelhecida:

- Chance de rejuvenescimento: **80%**
- Idade depois de rejuvenescer: **40**
- Restaurar saúde: **Ligado**
- Máximo de rejuvenescimentos por dia: **5.000**

Não comece com 100% + limite enorme. Isso pode transformar dezenas de milhares de aposentados em trabalhadores de uma vez e explodir o desemprego.

---

# PARTE 1 — Preparar o Windows

## 1. Atualizar Cities: Skylines II

Abra o Steam, atualize o **Cities: Skylines II** e abra o jogo pelo menos uma vez.

## 2. Instalar a ferramenta oficial de Code Modding

No Cities: Skylines II, procure a área de **Modding / Code Modding** e instale a toolchain oficial.

Depois que terminar, **feche o jogo**.

A toolchain deve disponibilizar a variável de ambiente:

```text
CSII_TOOLPATH
```

## 3. Verificar a toolchain

Abra o **PowerShell** e rode:

```powershell
$env:CSII_TOOLPATH
```

Se aparecer um caminho, ótimo.

Se não aparecer nada:

1. Feche o PowerShell.
2. Reinicie o Windows.
3. Abra o Cities: Skylines II novamente.
4. Confirme que as ferramentas de modding estão instaladas.
5. Feche o jogo.
6. Abra um PowerShell novo e teste novamente:

```powershell
$env:CSII_TOOLPATH
```

Não remova `Mod.props` nem `Mod.targets` do projeto para tentar contornar esse erro. O projeto depende deles.

---

# PARTE 2 — Instalar o .NET SDK

No PowerShell:

```powershell
dotnet --version
```

Se aparecer uma versão, por exemplo:

```text
8.0.xxx
9.0.xxx
10.0.xxx
```

pode continuar.

Se `dotnet` não for reconhecido, abra PowerShell/Terminal como Administrador e rode:

```powershell
winget install Microsoft.DotNet.SDK.10
```

Depois feche e abra o terminal novamente e confirme:

```powershell
dotnet --version
```

---

# PARTE 3 — Instalar o Git e baixar o projeto

Instale o Git:

```powershell
winget install Git.Git
```

Feche e abra o terminal novamente.

Crie uma pasta simples para seus mods:

```powershell
cd C:\
mkdir CS2Mods -ErrorAction SilentlyContinue
cd C:\CS2Mods
```

Clone este repositório:

```powershell
git clone https://github.com/decodxr/CimRejuvenator.git
cd CimRejuvenator
```

Como o repositório é público, normalmente não é necessário fazer login para clonar.

Confira os arquivos:

```powershell
dir
```

Você deve encontrar pelo menos:

```text
CimRejuvenator.csproj
Mod.cs
Setting.cs
RejuvenationSystem.cs
LocalePTBR.cs
LocaleEN.cs
build.ps1
check-environment.ps1
```

### Se preferir baixar ZIP

Na página do GitHub, use **Code > Download ZIP**, extraia para:

```text
C:\CS2Mods\CimRejuvenator
```

Depois abra o PowerShell nessa pasta.

---

# PARTE 4 — Verificar o ambiente automaticamente

Na pasta do projeto:

```powershell
.\check-environment.ps1
```

O ideal é aparecer algo parecido com:

```text
[OK] dotnet encontrado
[OK] CSII_TOOLPATH encontrado
[OK] Mod.props encontrado
[OK] Mod.targets encontrado
Ambiente parece pronto para compilar.
```

Se houver `[ERRO]`, resolva o item indicado antes de continuar.

---

# PARTE 5 — Compilar

Na pasta do projeto:

```powershell
.\build.ps1
```

Se o PowerShell bloquear scripts por `ExecutionPolicy`, libere somente para a janela atual:

```powershell
Set-ExecutionPolicy -Scope Process Bypass
```

Depois rode de novo:

```powershell
.\build.ps1
```

O resultado que você quer ver é:

```text
Build succeeded.
```

ou uma mensagem equivalente dizendo que a compilação terminou sem erros.

### Se quiser compilar sem o script

```powershell
dotnet build .\CimRejuvenator.csproj -c Release
```

---

# PARTE 6 — Onde o mod compilado vai parar

A toolchain do CS2 normalmente faz o deploy do mod para a pasta local de mods.

Abra `Win + R` e cole:

```text
%USERPROFILE%\AppData\LocalLow\Colossal Order\Cities Skylines II\Mods
```

Procure por algo relacionado a:

```text
CimRejuvenator
```

O `.dll` também deverá existir na saída de compilação do projeto, normalmente dentro de `bin\Release`.

---

# PARTE 7 — Primeiro teste no jogo

## 1. Faça backup do save

Não teste diretamente na única cópia da sua cidade.

Use algo assim:

```text
SAVE ORIGINAL -> guardar
SAVE TESTE    -> usar com o mod
```

## 2. Abra o Cities: Skylines II

Procure:

```text
Opções > Mods > Cim Rejuvenator
```

Configure inicialmente:

```text
Ativar mod:                     SIM
Chance de rejuvenescimento:     80%
Idade depois de rejuvenescer:   40
Restaurar saúde:                SIM
Máximo por dia:                 5000
```

Carregue a cidade pausada e só depois deixe a simulação rodar.

Comece em velocidade **1x**.

Observe:

- porcentagem de idosos;
- porcentagem de adultos;
- desemprego;
- mortes/mês;
- trabalhadores disponíveis;
- abandono de prédios.

Quando estiver estável, teste 2x. Evite começar direto em 3x.

---

# PARTE 8 — Como ajustar os valores

## Configuração recomendada

```text
Chance:       80%
Limite/dia:   5000
Idade:        40
Saúde:        ligada
```

## Se o desemprego explodir

Baixe o limite diário para:

```text
1500–2500
```

Você pode manter a chance em 80%; reduzir o limite diário deixa a correção mais lenta e suave.

## Se ainda estiver acontecendo uma death wave enorme

Temporariamente tente:

```text
7500–10000 por dia
```

Depois reduza novamente quando a população ficar mais equilibrada.

## Se quiser um modo quase imortal contra velhice

Use:

```text
Chance: 100%
```

Ainda mantenha um limite diário razoável.

Doenças e acidentes continuam podendo matar. O objetivo do mod é contornar principalmente a morte relacionada ao envelhecimento.

---

# PARTE 9 — Como atualizar o mod depois

Se você clonou com Git:

```powershell
cd C:\CS2Mods\CimRejuvenator
git pull
.\build.ps1
```

Depois abra o jogo novamente.

Fluxo:

```text
GitHub -> git pull -> build.ps1 -> Cities: Skylines II
```

---

# PARTE 10 — Erros comuns

## `dotnet` não é reconhecido

Instale:

```powershell
winget install Microsoft.DotNet.SDK.10
```

Feche e abra o terminal.

## `CSII_TOOLPATH` está vazio

Teste:

```powershell
$env:CSII_TOOLPATH
```

Se não aparecer nada, reinstale/atualize a Code Modding Toolchain do CS2 e reinicie o Windows.

## `Mod.props` não encontrado

Teste:

```powershell
Test-Path "$env:CSII_TOOLPATH\Mod.props"
```

Tem que retornar:

```text
True
```

## `Mod.targets` não encontrado

```powershell
Test-Path "$env:CSII_TOOLPATH\Mod.targets"
```

Também deve retornar `True`.

## Execução de scripts desabilitada

Na janela atual:

```powershell
Set-ExecutionPolicy -Scope Process Bypass
```

Depois:

```powershell
.\build.ps1
```

## Erros `CSxxxx` de namespace/classe do jogo

Primeiro rode:

```powershell
.\check-environment.ps1
```

Se tudo estiver `[OK]`, salve o erro completo:

```powershell
.\build.ps1 *> build-error.txt
```

Isso cria `build-error.txt` na pasta do projeto.

Como o Cities: Skylines II pode mudar sua API em atualizações, esse arquivo é importante para identificar exatamente o que precisa ser adaptado.

## O jogo abre, mas o mod não aparece

1. Confirme que a build terminou sem erros.
2. Confira a pasta de Mods.
3. Reinicie o jogo.
4. Confira se Code Mods estão habilitados.
5. Procure `CimRejuvenator` nos logs do jogo.

## O save ficou estranho

Não salve por cima do original.

Saia sem sobrescrever e volte para o backup.

---

# PARTE 11 — Resumo ultrarrápido

No Windows:

```powershell
winget install Git.Git
winget install Microsoft.DotNet.SDK.10

git clone https://github.com/decodxr/CimRejuvenator.git
cd CimRejuvenator

$env:CSII_TOOLPATH
.\check-environment.ps1
.\build.ps1
```

Depois no jogo:

```text
80% / idade 40 / saúde ligada / 5000 por dia
```

Comece em 1x, acompanhe idosos, adultos e desemprego, e só depois aumente a velocidade.
