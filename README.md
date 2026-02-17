# 📜 Diário de Desenvolvimento - Pokedéx (Sessão 04/02)

Este repositório documenta a evolução da aplicação Pokedéx desenvolvida em WPF com .NET 8. Hoje, os esforços foram concentrados na robustez da importação de dados e na implementação de uma arquitetura de navegação escalável.

## 🚀 Implementações Realizadas

### 1. Arquitetura de Navegação (MVVM)
* **MainViewModel**: Centralização da lógica de troca de contexto utilizando a propriedade `CurrentView`.
* **ContentControl Dinâmico**: Configuração do `MainWindow.xaml` para renderizar Views automaticamente com base no tipo do ViewModel presente no `CurrentView` via `DataTemplates`.
* **Refatoração de Comandos**: Implementação da versão não-genérica da classe `RelayCommand`. Isso permitiu o acionamento de comandos de navegação simples, eliminando erros de tipagem genérica (`requires 1 type arguments`) para cliques de botão sem parâmetros.



### 2. Engenharia de Dados e Sincronização
* **DatabaseSeeder Resiliente**:
    * **UniversalEnumConverter**: Criado para mapear strings complexas e algarismos romanos (comuns no `moves.json`) diretamente para tipos `Enum`.
    * **Flexible Converters**: Adição de conversores para `int` e `string` que tratam automaticamente valores nulos ou vazios (`""`) vindos do JSON, prevenindo quebras na carga do banco SQLite.
* **Lógica de Mapeamento de Imagens**:
    * Implementação de uma lógica de extração via `Path.GetFileName`. O sistema agora lê o caminho base fornecido no JSON e replica a estrutura para gerar automaticamente os caminhos de:
        * **Shiny**: `assets/pokemon_images/shiny/`
        * **Female**: `assets/pokemon_images/female/`
        * **Female Shiny**: `assets/pokemon_images/female_shiny/`
    * Suporte nativo para formas complexas como **Alcremie** (numeração dupla), **Formas Regionais**, **Megas (X/Y)** e **Gigantamax**.

### 3. Ajustes de Modelagem (Entity Framework)
* **JsonPropertyName**: Sincronização precisa entre os campos do JSON e as propriedades do C#, garantindo a captura de campos como `G-Max Move` (com hífen) e `Female_Shiny` (com underline).



## 🛠️ Tecnologias Utilizadas
* **WPF** (.NET 8)
* **Entity Framework Core** (SQLite)
* **CommunityToolkit.Mvvm** (Refatoração de Commands)
* **System.Text.Json** (Custom Converters)

## 📌 Status Atual e Próximos Passos
Atualmente, o banco de dados é populado com integridade total, e a navegação entre telas está funcional. O próximo marco será o refinamento da interface de usuário da Pokédex e a implementação de filtros de busca em tempo real.

---
*Atualizado em: 04 de Fevereiro de 2026*



# 📜 Diário de Desenvolvimento - Pokedéx (Sessão 05/02)


Este arquivo serve como um log diário de progresso, registrando as implementações técnicas, decisões de design e desafios superados.

---
## 🚀 Implementações Realizadas

### ✅ Novas Funcionalidades (UI/UX)
* **Navegação por Regiões:** * Substituição da barra de busca textual por uma **Barra de Filtros Horizontal**.
    * Implementação de lógica de filtragem por intervalo de Dex (ex: Kanto = #001-#151).
    * Botão "All" para visualizar a Pokédex completa (0-9999).
* **Identidade Visual Dinâmica:**
    * Adição de título dinâmico que muda conforme a região selecionada (ex: "Pokedex" ou "Johto").
    * Feedback visual (Hover) e Tooltips nos botões de filtro.

### 🛠️ Soluções Técnicas & Refatoração
* **Custom Font Icons (.ttf):**
    * Integração do arquivo `PokemonIcons.ttf` como **Resource** embarcado.
    * Mapeamento de ícones vetoriais via classe estática `CustomIcons` para evitar *magic strings*.
    * Uso de **Pack URIs** no XAML (`pack://application:,,,/Pokedex;component/...`) para garantir o carregamento da fonte em tempo de execução.
* **Solução Definitiva de Imagens:**
    * Correção do carregamento de assets locais usando `AbsoluteFilePathConverter` com `BitmapCacheOption.OnLoad`.
    * Configuração do `.csproj` para copiar a pasta `assets` automaticamente para o diretório de saída.
* **Layout Responsivo:**
    * Migração para **Scroll Vertical** com grid de cards auto-ajustável (`WrapPanel`).
    * Ajuste nas dimensões dos cards (140x190) para melhor densidade de informação.

### 🐛 Bugs Corrigidos
* **"Tofu" (Quadrados Vazios):** Resolvido alterando a *Build Action* da fonte para `Resource` e corrigindo o caminho no `FontFamily` do XAML.
* **XAML Parse Exception:** Correção na declaração dos *Resources* estáticos (`TypeToBrushConverter`) dentro da `PokedexView`.

---

## 📌 Resumo Técnico Acumulado
* **Arquitetura:** WPF (.NET 8) + MVVM.
* **Banco de Dados:** SQLite com Entity Framework Core.
* **Recursos:** Fonte de Ícones Vetoriais + Imagens Locais.
* **Pacotes:** `CommunityToolkit.Mvvm`, `System.Text.Json`.

---
*Próxima Etapa: Refinamento da Tela de Detalhes (DetailView).*

# 📜 Diário de Desenvolvimento - Pokedéx (Sessão 16/02)  

**Status:** Atualização Massiva de UI/UX e Legality Checker

## 📋 Resumo das Atualizações
O dia de hoje foi marcado por uma reestruturação profunda do aplicativo, transformando-o de uma simples Pokédex para uma **Single Page Application (SPA)** de gerenciamento competitivo de times no padrão 16:9 (1600x900). Foram implementadas regras rígidas de banco de dados, navegação contínua e um sistema de verificação de legalidade (Legality Checker) digno de softwares oficiais.

---

## 🐞 1. Correções do Banco de Dados e Visuais (Pokédex)
* **Refatoração do `DatabaseSeeder.cs`:** Removida a lógica que "inventava" caminhos de imagens para Pokémons Shiny e fêmeas que não existiam. O banco de dados agora respeita rigorosamente os campos vazios do JSON, corrigindo o bug onde a imagem normal era exibida no lugar da Shiny.
* **Navegação Contínua (Setas ◀ ▶):** Adicionados botões de navegação diretamente no cabeçalho do `PokemonDetailView`, permitindo pular para o próximo Pokémon ou o anterior da ordem numérica da Dex, ignorando formas alternativas e mega evoluções. Ocultação automática das setas nos limites (ex: Bulbasaur não mostra a seta para trás).
* **Navegação de Formas Alternativas:** Criados botões independentes para transitar entre Formas Alternativas (Megas, Regionals, etc) dentro do mesmo número de Dex.
* **Janela Estilo Dialog:** Removido via Windows API (`user32.dll`) o botão nativo de fechar ("X") do Windows no `PokemonDetailView`, forçando o uso do botão customizado da interface e tornando o título dinâmico com o nome do Pokémon atual.

---

## 🖥️ 2. Nova Estrutura de Navegação (Core UI)
* **Nova Resolução:** `MainWindow` redimensionada para a proporção **16:9 (1600x900)**, garantindo um visual *widescreen* moderno e muito mais espaço útil.
* **Splash Screen / Loading:** Fluxo de inicialização atualizado para exibir uma tela de Splash vibrante com a logo, a Pokébola e os créditos do desenvolvedor, transitando suavemente após 3 segundos.
* **Navegação por ViewModel:** Reestruturação da `MainWindow` com um `ContentControl` central e um menu superior moderno de navegação (`INÍCIO` e `VER POKÉDEX`), eliminando a abertura de múltiplas janelas (Popup) em prol de carregamento interno (UserControls).
* **Nova Tela Inicial (`HomeView`):** Criação de uma *landing page* minimalista, exibindo o título "P O K É M O N" estilizado e créditos de desenvolvimento.

---

## 🪪 3. Novo Módulo: Registro de Treinador (Trainer Card)
* **Design "Trainer Card":** Criação do `TrainerRegisterView` com um layout de cartão flutuante com sombras (DropShadow).
* **Cascading Dropdowns (Filtro Lógico):** Implementada lógica inteligente em C# atrelada ao Entity Framework:
  * O campo *Game* fica desativado até a *Generation* ser escolhida.
  * A *Generation* só libera as opções que pertencem à *Region* escolhida.
  * O *Game* cruza a Região e a Geração (Ex: Região Kanto + Geração 3 = Libera apenas *FireRed* e *LeafGreen*).
* **Database Integration:** Criado o modelo `Trainer.cs` e tabela adicionada ao `AppDbContext`.

---

## ⚔️ 4. Novo Módulo: Team Builder (Registro de Pokémon)
* **Layout Duplo Profissional:** Construção do `PokemonRegisterView` contendo dados de identificação na esquerda e um *Laboratório de Status* (Genetics & Training) na direita.
* **Legality Checker (Anti-Hack):**
  * O formulário **exige** a seleção de um Treinador primeiro.
  * Os Pokémons disponíveis na combobox são limitados pela Geração e Jogo do treinador (Ex: Treinador de Gen 1 não pode ver Pokémons acima da Dex 151).
  * O *Moveset* é filtrado para exibir apenas golpes que existiam até a Geração do treinador.
* **Separação Base / Form:** O usuário escolhe primeiro a Espécie Base (ex: Slowbro) e um segundo ComboBox carrega apenas as variantes daquela espécie (ex: Kantonian, Mega, Galarian).
* **Habilidades Dinâmicas:** O campo `Ability` varre o JSON e exibe apenas as habilidades possíveis da forma específica escolhida (identificando as Hidden Abilities - HA).
* **Estrela Shiny (Toggle):** Substituição de checkboxes clássicos por um botão de estrela `⭐` animado que brilha `🌟` ao ser ativado.
* **Sliders Sincronizados (Nível, IVs e EVs):** * Criação de controles deslizantes de "arrastar" mapeados em tempo real com caixas de texto via XAML Binding.
  * **Regra Competitiva:** Trava matemática (Hard-cap invisível) desenvolvida no Code-Behind para **não permitir que a soma total dos EVs ultrapasse 510**. Se o usuário tentar passar do limite, o slider bloqueia em tempo real.
  * Valores padrão atualizados: IVs começam em `0`, Nível começa no `50`.
* **Database Integration:** Criado o modelo `RegisteredPokemon.cs` para salvar o time diretamente amarrado à ID do Treinador no SQLite.

---

# 📜 Diário de Desenvolvimento - Pokedéx (Sessão 17/02)

Hoje foi um dia de grande avanço na arquitetura e nas funcionalidades principais do **Pokémon Builder / Pokédex**. O foco principal foi a estruturação visual e a transição definitiva para o padrão de design MVVM, garantindo um código limpo, reativo e livre de erros de interface.

## 🚀 O que foi feito hoje

* **Telas de Exibição (Read-Only):**
    * Criação da tela principal de exibição de **Treinadores**, listando as suas equipas ativas.
    * Desenvolvimento da tela de **Detalhes do Pokémon**, permitindo visualizar os status finais, habilidades, naturezas e moveset escolhidos pelo treinador.
* **Refatoração Profissional (MVVM):**
    * Limpeza completa dos ficheiros *Code-Behind* (`.xaml.cs`). Toda a lógica de negócio foi migrada para as respetivas `ViewModels`.
    * Implementação de `Bindings` reativos em todos os elementos da interface (ComboBoxes, TextBoxes, Sliders, ToggleButtons).
* **Regras de Negócio e Validações Inteligentes:**
    * **Anti-Duplicação de Moves:** Criada uma lógica dinâmica onde a seleção de um golpe o remove imediatamente das outras opções, impedindo a criação de *movesets* duplicados.
    * **Bloqueio de EVs (Effort Values):** O limite global de **510 pontos** (e 255 por atributo) foi blindado. Os sliders agora calculam matematicamente o máximo disponível e "travam" fisicamente a interface quando o limite é atingido.
    * **Seleção em Cascata Precisa:** A seleção do Treinador agora dita, de forma matemática e automática, o limite da Pokédex (ex: *Hisui = Dex 905*) e filtra os ataques permitidos até àquela Geração.
    * **Habilidades Reativas:** O campo `Ability` deixou de ser um texto livre e passou a ser uma ComboBox que puxa automaticamente as habilidades base e a *Hidden Ability* do Pokémon (e Forma) selecionado.

## 🔜 O que vamos fazer a seguir (Próximos Passos)

1.  **Novas Mega Evoluções:**
    * Adicionar suporte e dados para novas Mega Evoluções no banco de dados e garantir que a mecânica de mudança de stats e habilidades reflita corretamente no Builder.
2.  **Polimento Visual (UI/UX):**
    * Implementar efeitos de *Hover* (quando o rato passa por cima) nos botões principais, utilizando transições suaves para cores próximas às originais de cada botão (ex: Azul claro para o botão *Save*, Cinza escuro para o *Cancel*).
3.  **Clean Code Contínuo:**
    * Manter a vigilância na separação de responsabilidades (UI vs Lógica) e refatorar qualquer código remanescente que não esteja de acordo com os padrões rigorosos do MVVM.


## 🎨 Evolução do Design System

Hoje, a aplicação deixou de usar cores estáticas para adotar um sistema de **Recursos Dinâmicos**. Isso permite a troca de tema em tempo real.

### 🌑 Dark Mode & Cores Dinâmicas
Substituímos todos os valores fixos (como `White` ou `#F5F5F7`) por chaves de `DynamicResource`.
* **Fundos:** `AppBackground`, `ColorSurface` e `ColorSurfaceVariant`.
* **Tipografia:** `TextColorPrimary` (para títulos) e `TextColorSecondary` (para descrições).
* **Bordas:** `ColorBorder` para separadores e contornos sutis.

### 🧱 Componentes Padronizados
Criamos estilos globais no `App.xaml` para garantir consistência em todas as janelas:
* **MaterialButton:** Botões com cantos arredondados (10px a 15px) e feedback visual de *hover*.
* **MaterialComboBox:** Estilo limpo que abandona o visual nativo do Windows em favor de uma estética Material Design.
* **Campos de Input:** Agora brilham com a cor de acento do contexto (Amarelo para Treinadores, Vermelho para Pokedex).

---

## 🧠 Refatoração Estrutural (MVVM)

A janela `RegisteredPokemonDetailView` foi o nosso maior desafio e sucesso do dia.

* **Arquitetura:** Migrada de Code-behind puro para **MVVM**.
* **ViewModel:** Criada a `RegisteredPokemonDetailViewModel` para processar:
    * Cálculos matemáticos dos Status (HP, Atk, Def, etc.).
    * Lógica de navegação entre a equipa do treinador.
    * Formatação de strings (ex: colocar "-" em movimentos vazios).
* **Gráfico de Radar:** Implementação de três modos de visualização dinâmicos:
    1.  **Calculated Stats:** Deep Purple 200/A400.
    2.  **EVs (Effort Values):** Cyan 200/A400.
    3.  **IVs (Individual Values):** Orange 200/A400.

---

## 🛠️ UX & Ajustes Técnicos

* **Trainer ID:** Implementada validação no evento `PreviewTextInput` para aceitar apenas números e limite de 6 caracteres.
* **LoadingView:** Adicionado efeito de transparência (`Opacity="0.90"`) para um visual de *splash screen* moderno.
* **Fix MultiBinding:** Resolvida a limitação do `PointCollection` no XAML através do uso de `Styles` para injetar os pontos do polígono.
* **AbsoluteFilePathConverter:** Padronizado o carregamento de imagens em todas as Views para evitar erros de caminho de ficheiro.

---

## 📅 Próximos Passos
- [ ] Implementar a persistência do tema (guardar a preferência do utilizador num ficheiro config).
- [ ] Criar animações de transição suave entre os cartões da Pokedex.
- [ ] Padronizar a janela de diálogo de erros com o novo design.

> **Status Atual:** 🟢 Funcional e Modernizado.

*Development by: [Tiago Guerino de Oliveira Bassani] - Projeto C# WPF & SQLite*