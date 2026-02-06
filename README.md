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