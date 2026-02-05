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
