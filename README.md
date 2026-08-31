# Cim Rejuvenator v0.1.2

> **Status: experimental / first public test build.** Faça backup do save antes de usar. O projeto foi criado para Cities: Skylines II e pode precisar de pequenos ajustes caso a API do jogo mude.

Mod para **Cities: Skylines II** que transforma uma porcentagem configurável dos cidadãos idosos em adultos novamente, preservando o mesmo cidadão.

## 🚀 Comece aqui

Se você vai instalar/compilar no Windows e quer um passo a passo completo, abra:

**[TUTORIAL-WINDOWS.md](TUTORIAL-WINDOWS.md)**

Também existe um modo **SEM Unity / SEM ativação de licença**, pensado para compilar localmente usando diretamente as DLLs instaladas do jogo.

### Build sem Unity

```powershell
.\check-environment.ps1
.\build-no-unity.ps1
```

Se o jogo estiver em um caminho diferente do padrão:

```powershell
$env:CSII_GAMEPATH="D:\SteamLibrary\steamapps\common\Cities Skylines II"
.\build-no-unity.ps1
```

Esse modo ignora a toolchain oficial e referencia diretamente:

```text
Cities2_Data\Managed\Game.dll
Cities2_Data\Managed\Unity.Entities.dll
Colossal.*.dll
...
```

O resultado pronto para copiar fica em:

```text
dist\CimRejuvenator\CimRejuvenator.dll
```

> Para publicação oficial no Paradox Mods, ainda é melhor usar a toolchain oficial. O fallback sem Unity é para build/teste local.

## O que o mod faz

- Interface em **Opções > Mods > Cim Rejuvenator**.
- Slider de **0% a 100%** para chance de rejuvenescimento.
- O mesmo cidadão volta de idoso para adulto; a ideia é preservar família, casa, educação e identidade.
- O sorteio não fica se repetindo a cada frame durante o mesmo ciclo de velhice.
- Se o cidadão rejuvenescido envelhecer novamente no futuro, ele pode participar de um novo sorteio.
- Redefine a idade interna/data de nascimento para corresponder à idade configurada.
- Pode restaurar a saúde mínima.
- Cidadãos já mortos não são ressuscitados.
- Doença e acidente continuam podendo matar.
- Limite diário evita converter uma cidade inteira de uma vez.

## Configuração inicial recomendada

Para uma cidade extremamente envelhecida (~92% idosos):

- Chance: **80%**
- Idade após rejuvenecer: **40**
- Restaurar saúde: **Ligado**
- Máximo por dia: **5.000**

Se o desemprego subir demais, reduza o limite para **1.500–2.500/dia**.

Se a onda de mortes continuar muito forte, tente temporariamente **7.500–10.000/dia**.

## Baixar/clonar

```powershell
git clone https://github.com/decodxr/CimRejuvenator.git
cd CimRejuvenator
```

## Compilar

### Sem Unity (recomendado para teste local)

```powershell
.\check-environment.ps1
.\build-no-unity.ps1
```

### Toolchain oficial

```powershell
.\check-environment.ps1
.\build.ps1
```

## Windows → Linux / Proton

Depois do build sem Unity, copie a pasta:

```text
dist\CimRejuvenator
```

para o Linux e coloque na pasta de mods do prefixo Proton do Cities: Skylines II, normalmente algo como:

```text
~/.local/share/Steam/steamapps/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II/Mods/CimRejuvenator/
```

ou:

```text
~/.steam/steam/steamapps/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II/Mods/CimRejuvenator/
```

## Atualizar pelo GitHub

Depois de clonar o projeto uma vez:

```powershell
cd C:\CS2Mods\CimRejuvenator
git pull
.\build-no-unity.ps1
```

## Aviso importante

**Faça backup do save antes do primeiro teste.** O mod altera componentes de cidadãos em uma simulação existente. Não trate a alteração como perfeitamente reversível.

## Licença

MIT — veja [LICENSE](LICENSE).
