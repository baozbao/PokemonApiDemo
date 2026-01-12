using DemoApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DemoApi.Infrastructure.Data
{
    public static class PokemonSeeder
    {
        public static void Seed(PokemonDbContext context)
        {
            if (context.Pokemons.Any())
            {
                Console.WriteLine("Pokemon data already exists. Skipping seed. ⏩");
                return;
            }

            Console.WriteLine("Planting Waifu Seeds... 🌱");

            var waifus = new List<Pokemon>
            {
                // --- The Eeveelution Family (All Female) ---
                CreatePokemon("Eevee", "Evolution Pokemon", "Female", 5, "Normal"),
                CreatePokemon("Vaporeon", "Bubble Jet Pokemon", "Female", 15, "Water"),
                CreatePokemon("Jolteon", "Lightning Pokemon", "Female", 15, "Electric"),
                CreatePokemon("Flareon", "Flame Pokemon", "Female", 15, "Fire"),
                CreatePokemon("Espeon", "Sun Pokemon", "Female", 20, "Psychic"),
                CreatePokemon("Umbreon", "Moonlight Pokemon", "Female", 20, "Dark"),
                CreatePokemon("Leafeon", "Verdant Pokemon", "Female", 20, "Grass"),
                CreatePokemon("Glaceon", "Fresh Snow Pokemon", "Female", 20, "Ice"),
                CreatePokemon("Sylveon", "Intertwining Pokemon", "Female", 25, "Fairy"),

                // --- The Queens & Princesses ---
                CreatePokemon("Meowscarada", "Magician Pokemon", "Female", 36, "Grass/Dark"),
                CreatePokemon("Gardevoir", "Embrace Pokemon", "Female", 30, "Psychic/Fairy"),
                CreatePokemon("Lopunny", "Rabbit Pokemon", "Female", 25, "Normal", isReleased: true), // 即使在电脑里也可能被放生了
                CreatePokemon("Braixen", "Fox Pokemon", "Female", 16, "Fire"),
                CreatePokemon("Delphox", "Fox Pokemon", "Female", 36, "Fire/Psychic"),
                CreatePokemon("Primarina", "Soloist Pokemon", "Female", 34, "Water/Fairy"),
                CreatePokemon("Hatterene", "Silent Pokemon", "Female", 42, "Psychic/Fairy"),
                CreatePokemon("Tsareena", "Fruit Pokemon", "Female", 29, "Grass"),
                CreatePokemon("Lilligant", "Flowering Pokemon", "Female", 25, "Grass"),
                CreatePokemon("Milotic", "Tender Pokemon", "Female", 30, "Water"),
                CreatePokemon("Roserade", "Bouquet Pokemon", "Female", 25, "Grass/Poison"),
                CreatePokemon("Gothitelle", "Astral Body Pokemon", "Female", 41, "Psychic"),
                CreatePokemon("Mawile", "Deceiver Pokemon", "Female", 20, "Steel/Fairy"),
                CreatePokemon("Ninetales (Alolan)", "Fox Pokemon", "Female", 20, "Ice/Fairy"),
                CreatePokemon("Froslass", "Snow Land Pokemon", "Female", 25, "Ice/Ghost"),
                CreatePokemon("Leavanny", "Nurturing Pokemon", "Female", 25, "Bug/Grass"),
                CreatePokemon("Salazzle", "Toxic Lizard Pokemon", "Female", 33, "Poison/Fire"),
                CreatePokemon("Pheromosa", "Lissome Pokemon", "Female", 50, "Bug/Fighting"), 
                CreatePokemon("Meloetta", "Melody Pokemon", "Female", 50, "Normal/Psychic"),
                CreatePokemon("Diancie", "Jewel Pokemon", "Female", 50, "Rock/Fairy"),
                CreatePokemon("Tinkaton", "Hammer Pokemon", "Female", 38, "Fairy/Steel"),

                // --- Some Husbandos for Balance (Males) ---
                CreatePokemon("Lucario", "Aura Pokemon", "Male", 30, "Fighting/Steel"),
                CreatePokemon("Garchomp", "Mach Pokemon", "Male", 48, "Dragon/Ground"),
                CreatePokemon("Charizard", "Flame Pokemon", "Male", 36, "Fire/Flying"),
                CreatePokemon("Greninja", "Ninja Pokemon", "Male", 36, "Water/Dark"),
                CreatePokemon("Zoroark", "Illusion Pokemon", "Male", 30, "Dark"),
                CreatePokemon("Gallade", "Blade Pokemon", "Male", 30, "Psychic/Fighting"),
                CreatePokemon("Arcanine", "Legendary Pokemon", "Male", 25, "Fire"),
                CreatePokemon("Incineroar", "Heel Pokemon", "Male", 34, "Fire/Dark"),
                CreatePokemon("Toxtricity", "Punk Pokemon", "Male", 30, "Electric/Poison"),

                // --- The Mascots ---
                CreatePokemon("Pikachu", "Mouse Pokemon", "Male", 5, "Electric"),
                CreatePokemon("Mimikyu", "Disguise Pokemon", "Female", 20, "Ghost/Fairy"),
            };

            context.Pokemons.AddRange(waifus);
            context.SaveChanges();
            Console.WriteLine($"Added {waifus.Count} Pokemon Waifus 🎉");
        }

        private static Pokemon CreatePokemon(string name, string specie, string gender, int level, string type, bool isReleased = false)
        {
            return new Pokemon
            {
                Name = name,
                Specie = specie,
                Gender = gender,
                Level = level,
                Type = type,
                IsInTeam = false, // 默认都在电脑里
                IsReleased = isReleased,
                GuidId = Guid.NewGuid()
            };
        }
    }
}
