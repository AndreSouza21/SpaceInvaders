# Space Invaders — edição Minecraft

Um Space Invaders feito em C# (Windows Forms) com tema de Minecraft: você é o Steve, os inimigos são Herobrines que atiram bolas de fogo de Ghast, e os escudos vão quebrando conforme levam dano.

## Como jogar

| Tecla | Ação |
|-------|------|
| `A` / `D` | Mover para esquerda / direita |
| `Espaço` | Atirar |
| `B` | Usar o especial (quando carregado) |
| `Esc` | Pausar / continuar |

- Colete o **totem** que cai para carregar o **especial**.
- O especial interage tanto com os **escudos** quanto com os **inimigos**.
- Pegue os **corações** para recuperar vida.

## Baixar e jogar (sem instalar nada)

1. Vá em **[Releases](../../releases)** e baixe o `SpaceInvaders-win.zip`.
2. Extraia o zip (mantenha a pasta `Assets` ao lado do `.exe`).
3. Execute `SpaceInvaders.exe`.

Funciona no Windows 10/11 — usa o .NET Framework 4.7.2, que já vem com o sistema.

## Compilar a partir do código

Requisitos: Windows + [.NET SDK](https://dotnet.microsoft.com/download) (ou Visual Studio 2022).

```bash
git clone https://github.com/SEU_USUARIO/SpaceInvaders.git
cd SpaceInvaders
dotnet run --project src/SpaceInvaders
```

Ou abra `SpaceInvaders.sln` no Visual Studio e aperte **F5**.

## Estrutura

```
src/SpaceInvaders/   código-fonte e Assets (imagens e música)
design/              arquivos-fonte de arte (.psd)
.github/workflows/   build e release automáticos
```

## Créditos e direitos autorais

Projeto de fã, sem fins lucrativos, **sem afiliação com a Mojang ou a Microsoft**. Minecraft é marca da Mojang AB.
Os personagens/texturas inspirados em Minecraft e a música *Moog City* (C418) pertencem aos seus respectivos autores; a música é opcional — se o arquivo `Assets/C418-Moog-City.wav` não existir, o jogo roda sem som.

O código-fonte está sob a licença [MIT](LICENSE).
