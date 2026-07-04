# Gatinha Intergaláctica - Guia de Setup da Fase de Teste

Este documento explica como montar a fase de teste do jogo usando os scripts criados.

## 📋 Scripts Criados

1. **HazardKill.cs** - Obstáculo letal que reinicia a fase ao toque
2. **EnemyPatrol.cs** - Alien que patrulha horizontalmente
3. **Projectile.cs** - Projétil disparado automaticamente
4. **CameraFollow.cs** - Câmera que segue a gatinha
5. **PlayerController.cs** - Atualizado com disparo automático

## 🎮 Setup Passo a Passo

### 1. Configurar Tags e Layers

Primeiro, configure as tags necessárias no Unity:
- **Edit → Project Settings → Tags and Layers**
- Adicione as seguintes tags se ainda não existirem:
  - `Player`
  - `Enemy`
  - `Projectile`
  - `Ground` (para plataformas)
  - `Hazard` (opcional, para meteoros)

### 2. Criar o Prefab do Projétil

1. **Hierarchy → Create Empty GameObject** → nomeie como "Projectile"
2. Adicione um **Sprite Renderer**:
   - Sprite: use um quadrado pequeno pixel art ou sprite existente
   - Color: amarelo ou branco brilhante
   - Sorting Layer: coloque acima do player
3. Adicione **BoxCollider2D**:
   - ✅ Is Trigger
   - Ajuste o tamanho para cobrir o sprite
4. Adicione **Rigidbody2D**:
   - Body Type: Kinematic
   - Gravity Scale: 0
5. Adicione o script **Projectile.cs**
6. Configure no Inspector:
   - Speed: 10
   - Lifetime: 3
7. **Tag**: defina como "Projectile"
8. Arraste para a pasta **Assets/Prefabs/** para criar o prefab
9. Delete da Hierarchy

### 3. Criar o Prefab do Meteoro (Hazard)

1. **Hierarchy → Create Empty GameObject** → nomeie como "Meteoro"
2. Adicione um **Sprite Renderer**:
   - Sprite: selecione `Assets/Art/Decoração/12.png` (o meteoro)
   - Sorting Layer: acima do background
3. Adicione **PolygonCollider2D** ou **CircleCollider2D**:
   - ✅ Is Trigger
   - Ajuste para cobrir a área visível do meteoro
4. Adicione o script **HazardKill.cs**
5. Configure no Inspector:
   - Scene Name: "SampleScene" (ou o nome da sua cena)
6. **Tag**: defina como "Hazard" (opcional)
7. Arraste para a pasta **Assets/Prefabs/** para criar o prefab
8. Delete da Hierarchy

### 4. Criar o Prefab do Alien (Enemy)

1. **Hierarchy → Create Empty GameObject** → nomeie como "Alien"
2. Adicione um **Sprite Renderer**:
   - Sprite: use um sprite da pasta Decoração/ ou crie um placeholder
   - Sorting Layer: mesmo do player
3. Adicione **BoxCollider2D**:
   - ✅ Is Trigger
   - Ajuste o tamanho
4. Adicione **Rigidbody2D**:
   - Body Type: Kinematic
   - Gravity Scale: 0
5. Adicione o script **EnemyPatrol.cs**
6. Configure no Inspector:
   - Patrol Distance: 4 (ajuste conforme o tamanho da plataforma)
   - Speed: 2
   - Scene Name: "SampleScene"
7. **Tag**: defina como "Enemy"
8. Arraste para a pasta **Assets/Prefabs/** para criar o prefab
9. Delete da Hierarchy

### 5. Atualizar o Player

1. Selecione o GameObject do **Player** na cena
2. No Inspector, localize o script **PlayerController**
3. Configure a nova seção **Ataque**:
   - **Projectile Prefab**: arraste o prefab do projétil aqui
   - **Fire Rate**: 0.6 (um tiro a cada 0.6 segundos)
   - **Projectile Offset**: (0.5, 0) - ajuste conforme necessário

### 6. Configurar a Câmera

1. Selecione a **Main Camera** na Hierarchy
2. Adicione o script **CameraFollow.cs**
3. Configure no Inspector:
   - **Target**: arraste o GameObject do Player aqui
   - **Smooth Speed**: 0.125
   - **Offset**: (0, 2, -10)
   - **Use Bounds**: ☐ (desmarque para a fase de teste)

### 7. Montar a Fase na Cena "Basico Teste"

#### Background
1. **Hierarchy → Create → 2D Object → Sprite**
2. Nomeie como "Background"
3. **Sprite Renderer**:
   - Sprite: selecione uma imagem de `Assets/Art/Cenários/`
   - Sorting Layer: crie um layer "Background" e coloque ele primeiro
4. Posicione atrás de tudo e ajuste o Scale para cobrir a tela

#### Plataformas (Chão)
1. Use os tiles já fatiados em `Assets/Art/Chão/`
2. Opção A: **Tilemap**
   - Hierarchy → 2D Object → Tilemap → Rectangular
   - Window → 2D → Tile Palette
   - Crie uma paleta e adicione os tiles do chão
   - Pinte as plataformas
3. Opção B: **Prefabs de Plataforma**
   - Use os prefabs existentes de plataforma
   - Duplique e posicione para criar o percurso

#### Posicionar Meteoros
1. Arraste o prefab **Meteoro** da pasta Prefabs para a cena
2. Posicione sobre plataformas ou no caminho do player
3. Você pode duplicá-lo (Ctrl+D) para criar múltiplos obstáculos
4. **Dica**: Posicione alguns no ar para criar obstáculos de "esquiva no pulo"

#### Posicionar Aliens
1. Arraste o prefab **Alien** da pasta Prefabs para a cena
2. Posicione sobre uma plataforma
3. **Importante**: Ajuste o **Patrol Distance** no Inspector para que o alien não caia da plataforma
4. Crie 1-2 aliens para a fase de teste

#### Ponto Final (LevelEnd)
1. Você já deve ter um GameObject com o script **LevelEnd.cs** na cena
2. Posicione ele no final do percurso
3. Certifique-se de que tem um **Collider2D** com **Is Trigger** ativado

### 8. Criar UI de Vitória

1. **Hierarchy → UI → Canvas** (se ainda não existir)
2. Dentro do Canvas, crie um **Panel** → nomeie "VictoryPanel"
3. Dentro do VictoryPanel:
   - **UI → Text - TextMeshPro** → nomeie "VictoryText"
   - Texto: "VOCÊ VENCEU!"
   - Font Size: grande (60-80)
   - Alignment: Center
   - Color: amarelo ou branco brilhante
4. Adicione um **UI → Button - TextMeshPro** → nomeie "RestartButton"
   - Text: "Jogar Novamente"
   - OnClick(): arraste o objeto **LevelEnd** e selecione `LevelEnd.RestartGame()`
5. **Importante**: Desative o VictoryPanel no Inspector (☐) - ele será ativado automaticamente pelo script LevelEnd quando o player chegar ao fim

### 9. Configurar Cena no Build Settings

1. **File → Build Settings**
2. Certifique-se de que a cena está na lista "Scenes In Build"
3. Anote o nome exato da cena (ex: "SampleScene" ou "Basico Teste")
4. Use esse nome nos campos "Scene Name" dos scripts HazardKill e EnemyPatrol

## ✅ Checklist Final

Antes de testar, verifique:

- [ ] Player tem o prefab do projétil configurado no PlayerController
- [ ] Câmera tem o CameraFollow com target apontando para o Player
- [ ] Player tem tag "Player"
- [ ] Alien tem tag "Enemy"
- [ ] Projétil tem tag "Projectile"
- [ ] Plataformas têm Collider2D (não trigger) e tag "Ground"
- [ ] Meteoros têm Collider2D (trigger) e script HazardKill
- [ ] Aliens têm Collider2D (trigger), Rigidbody2D (Kinematic) e script EnemyPatrol
- [ ] LevelEnd está no final da fase com Collider2D (trigger)
- [ ] VictoryPanel está desativado no Canvas
- [ ] Nome da cena está correto nos scripts

## 🎯 Testando a Fase

1. **Play** no Unity
2. Teste cada mecânica:
   - ✅ Player anda e pula normalmente
   - ✅ Projéteis são disparados automaticamente
   - ✅ Projéteis destroem aliens ao acertar
   - ✅ Tocar meteoro reinicia a fase
   - ✅ Tocar alien reinicia a fase
   - ✅ Câmera segue o player suavemente
   - ✅ Chegar ao LevelEnd mostra tela de vitória

## 🐛 Troubleshooting

### Projétil não aparece
- Verifique se o prefab está configurado no PlayerController
- Veja se há erros no Console
- Certifique-se de que o Sorting Layer do projétil está visível

### Alien não morre ao ser atingido
- Verifique se o alien tem tag "Enemy"
- Verifique se o projétil tem tag "Projectile"
- Veja se ambos têm Collider2D com Is Trigger ativado

### Fase não reinicia ao tocar meteoro/alien
- Verifique o nome da cena nos scripts HazardKill e EnemyPatrol
- Veja se o meteoro/alien tem Collider2D com Is Trigger
- Confirme que o Player tem tag "Player"

### Câmera não segue o player
- Verifique se o target está configurado no CameraFollow
- Veja se a câmera está posicionada corretamente (Z negativo)

## 🎨 Melhorias Opcionais

Depois que tudo estiver funcionando, você pode adicionar:

1. **Efeitos Visuais**:
   - Particle System quando alien é destruído
   - Trail Renderer no projétil
   - Animação de explosão para o meteoro

2. **Som**:
   - Som de tiro
   - Som de explosão ao destruir alien
   - Som de morte ao tocar hazard
   - Música de fundo

3. **Mais Variedade**:
   - Meteoros que caem do céu (adicione Rigidbody2D com gravidade)
   - Aliens mais rápidos ou com patrulha maior
   - Power-ups temporários

4. **Polish**:
   - Fade in/out entre transições
   - Tela de pausa (ESC)
   - Contador de aliens derrotados

## 📚 Estrutura de Pastas Recomendada

```
Assets/
├── Art/
│   ├── Animation/
│   ├── Cenários/
│   ├── Chão/
│   ├── Decoração/
│   └── Personagem/
├── Prefabs/
│   ├── Projectile.prefab
│   ├── Meteoro.prefab
│   └── Alien.prefab
├── Scenes/
│   └── Basico Teste.unity
└── Scripts/
    ├── CameraFollow.cs
    ├── Collectible.cs
    ├── EnemyPatrol.cs
    ├── ESP32SerialReader.cs
    ├── GameManager.cs
    ├── HazardKill.cs
    ├── LevelEnd.cs
    ├── MovingPlatform.cs
    ├── PlayerController.cs
    └── Projectile.cs
```

## 🚀 Próximos Passos

Depois de completar a fase de teste:
1. Ajuste o balanceamento (velocidades, taxa de tiro, dificuldade)
2. Adicione mais fases com layouts diferentes
3. Implemente sistema de pontuação
4. Adicione diferentes tipos de inimigos
5. Crie um menu principal

---

Boa sorte com o desenvolvimento! 🐱🚀✨
