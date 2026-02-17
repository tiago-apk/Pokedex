namespace Pokedex.models
{
    public class DisplayPokemon
    {
        public int Id { get; set; }
        public string Dex { get; set; }
        public string Species { get; set; }
        public string DisplayName { get; set; }
        public int Level { get; set; }
        public string ImagePath { get; set; }
        public bool IsShiny { get; set; }
    }
}