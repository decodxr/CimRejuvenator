# TUTORIAL LINUX — Cim Rejuvenator v0.2.0

Agora o Cim Rejuvenator pode ser **compilado diretamente no Linux**, sem Unity e sem precisar voltar ao Windows.

O script usa as DLLs da instalação do Cities: Skylines II e pode copiar a DLL pronta diretamente para o prefixo Proton.

> Faça backup do save antes de testar uma versão nova.

---

## 1 — Instalar Git e .NET SDK

### Arch / Caelestia

```bash
sudo pacman -S git dotnet-sdk
```

Confirme:

```bash
git --version
dotnet --version
```

---

## 2 — Clonar ou atualizar o projeto

Primeira vez:

```bash
cd ~
git clone https://github.com/decodxr/CimRejuvenator.git
cd CimRejuvenator
```

Se já clonou:

```bash
cd ~/CimRejuvenator
git pull
```

---

## 3 — Tornar o script executável

```bash
chmod +x build-no-unity-linux.sh
```

---

## 4 — Build simples

```bash
./build-no-unity-linux.sh
```

O script tenta encontrar automaticamente o jogo nestes locais comuns:

```text
~/.local/share/Steam/steamapps/common/Cities Skylines II
~/.steam/steam/steamapps/common/Cities Skylines II
Steam Flatpak
```

Se o jogo estiver em outra Steam Library:

```bash
export CSII_GAMEPATH="/caminho/para/steamapps/common/Cities Skylines II"
./build-no-unity-linux.sh
```

A DLL pronta fica em:

```text
dist/CimRejuvenator/CimRejuvenator.dll
```

---

## 5 — Build + instalação automática no Proton

Com o Cities: Skylines II **fechado**, rode:

```bash
./build-no-unity-linux.sh --deploy
```

O script compila e tenta copiar automaticamente para:

```text
~/.local/share/Steam/steamapps/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II/Mods/CimRejuvenator/CimRejuvenator.dll
```

Depois é só abrir o jogo.

---

## 6 — Confirmar que carregou

```bash
grep -Rni "CimRejuvenator" \
"$HOME/.local/share/Steam/steamapps/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II/Logs" \
| tail -100
```

O log próprio normalmente fica em:

```text
.../Cities Skylines II/Logs/CimRejuvenator.log
```

Na v0.2.0 você deve ver algo semelhante a:

```text
Loading Cim Rejuvenator v0.2.0
RejuvenationSystem created...
Completed automatic rejuvenation sweep: ...
```

---

## 7 — Teste rápido da v0.2.0

Em:

```text
Opções > Mods > Cim Rejuvenator
```

para um teste agressivo:

```text
Chance:                    100%
Máximo por dia:         100.000
Máximo por varredura:    50.000
Varreduras por dia:          64
```

Pressione:

```text
REJUVENESCER AGORA
```

Feche as opções, despause o jogo e aguarde a simulação rodar.

O botão ainda respeita os limites configurados.

---

## 8 — Limites máximos atuais

```text
Máximo por dia:       250.000
Máximo por varredura: 100.000
Varreduras por dia:       256
```

Valores extremos existem principalmente para recuperação/testes. Para uso contínuo é melhor baixar depois.

---

## 9 — Configuração recomendada depois da recuperação

```text
Chance:                       80%
Idade:                         40
Saúde mínima:              ligada
Máximo por dia:        10k–25k
Máximo por varredura:   2k–5k
Varreduras por dia:            64
Manter mínimo de idosos:   ligado
Mínimo de idosos:          15–20%
```

---

## 10 — Atualizações futuras ficaram simples

Depois disso, sempre que o GitHub receber uma atualização:

```bash
cd ~/CimRejuvenator
git pull
./build-no-unity-linux.sh --deploy
```

E pronto: você não precisa mais entrar no Windows só para recompilar o mod.
