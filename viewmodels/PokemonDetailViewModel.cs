using Pokedex.models;
using Pokedex.viewmodels;
using System;
using System.Collections.Generic;
using System.Linq;

public class PokemonDetailViewModel : BaseViewModel
{
    private readonly List<PokedexEntry> _allForms;
    private int _currentFormIndex = 0;

    public PokemonDetailViewModel(List<PokedexEntry> forms)
    {
        _allForms = forms ?? new List<PokedexEntry>();
    }

    public PokedexEntry CurrentForm => _allForms.ElementAtOrDefault(_currentFormIndex);
    public PokedexEntry BaseForm => _allForms.FirstOrDefault();

    public string Name => !string.IsNullOrEmpty(CurrentForm?.Name) ? CurrentForm.Name : BaseForm?.Name;

    // Tratamento seguro para a Dex Number
    public string BaseDexNumber => CurrentForm?.Dex?.Split('.')[0] ?? "000";

    public List<string> Types
    {
        get
        {
            // Filtra os tipos para não exibir o PokemonType.None que o seeder gera para strings vazias
            var source = (CurrentForm?.Types != null && CurrentForm.Types.Any(t => t != PokemonType.None))
                         ? CurrentForm.Types
                         : BaseForm?.Types ?? new List<PokemonType>();

            return source.Where(t => t != PokemonType.None)
                         .Select(t => t.ToString())
                         .ToList();
        }
    }

    // Stats com herança da BaseForm se o valor for zero
    public int HP => (CurrentForm?.Hp > 0) ? CurrentForm.Hp : (BaseForm?.Hp ?? 0);
    public int ATK => (CurrentForm?.Atk > 0) ? CurrentForm.Atk : (BaseForm?.Atk ?? 0);
    public int DEF => (CurrentForm?.Def > 0) ? CurrentForm.Def : (BaseForm?.Def ?? 0);
    public int SPA => (CurrentForm?.Spa > 0) ? CurrentForm.Spa : (BaseForm?.Spa ?? 0);
    public int SPD => (CurrentForm?.Spd > 0) ? CurrentForm.Spd : (BaseForm?.Spd ?? 0);
    public int SPE => (CurrentForm?.Spe > 0) ? CurrentForm.Spe : (BaseForm?.Spe ?? 0);
    public string Image => CurrentForm?.Image ?? BaseForm?.Image;
    public string Shiny => CurrentForm?.Shiny ?? BaseForm?.Shiny;
    public string Female => CurrentForm?.Female ?? BaseForm?.Female;
    public string FemaleShiny => CurrentForm?.FemaleShiny ?? BaseForm?.FemaleShiny;

    public string HasMultipleForms => _allForms.Count > 1 ? "Visible" : "Collapsed";

    public void NextForm()
    {
        if (_allForms.Count <= 1) return;
        _currentFormIndex = (_currentFormIndex + 1) % _allForms.Count;
        NotifyAll();
    }

    public void PreviousForm()
    {
        if (_allForms.Count <= 1) return;
        _currentFormIndex = (_currentFormIndex - 1 + _allForms.Count) % _allForms.Count;
        NotifyAll();
    }

    private void NotifyAll() => OnPropertyChanged(string.Empty);
}