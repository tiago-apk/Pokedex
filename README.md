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
*Development by: [Tiago Guerino de Oliveira Bassani] - Projeto C# WPF & SQLite*