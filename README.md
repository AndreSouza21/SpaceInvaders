# Space Invaders — edição Minecraft

Projeto acadêmico desenvolvido para uma disciplina da faculdade, em que o tema era livre. É um Space Invaders feito em C# (Windows Forms) com tema de Minecraft: você é o Steve, os inimigos são Herobrines que atiram bolas de fogo de Ghast, e os escudos vão quebrando conforme levam dano.

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

1. Vá em **[Releases](https://github.com/AndreSouza21/SpaceInvaders/releases)** e baixe o `SpaceInvaders-win.zip`.
2. Extraia o zip (mantenha a pasta `Assets` ao lado do `.exe`).
3. Execute `SpaceInvaders.exe`.

Funciona no Windows 10/11 — usa o .NET Framework 4.7.2, que já vem com o sistema.

## Compilar a partir do código

Requisitos: Windows + [.NET SDK](https://dotnet.microsoft.com/download) (ou Visual Studio 2022).

```bash
git clone https://github.com/AndreSouza21/SpaceInvaders.git
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

## Aviso legal

Este é um projeto de fã, feito exclusivamente para fins educacionais e sem qualquer fim comercial. Não possui afiliação, patrocínio ou aprovação da Mojang Studios, da Microsoft ou de qualquer outro detentor de direitos.

*Minecraft* é marca registrada da Mojang AB. Os personagens, texturas e elementos visuais inspirados em Minecraft, bem como a música *Moog City* (C418), pertencem aos seus respectivos autores e são usados aqui apenas de forma ilustrativa, sem intenção de violar direitos autorais. A música é opcional: sem o arquivo `Assets/C418-Moog-City.wav`, o jogo roda normalmente, sem som.

Caso algum detentor de direitos queira que algum material seja removido deste repositório, basta abrir uma *issue* ou entrar em contato que a remoção será feita.