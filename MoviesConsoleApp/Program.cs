using Microsoft.EntityFrameworkCore;
using Persistencia.Entidades;
using Persistencia.Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MoviesConsoleApp
{
    class Program
    {
        static void Main(String[] args)
        {
            // acesso ao EF serah realizado pela variavel _db
            // essa dependencia da camada de apresentacao com
            // a camada de persistencia eh errada!
            MovieContext _db = new MovieContext();

            Console.WriteLine();
            Console.WriteLine("1. Listar o nome de todos personagens desempenhados por um determinado ator, incluindo a informação de qual o título do filme e o diretor");

            var querry1 = from per in _db.Characters
                                             .Include(a => a.Actor)
                                             .Include(m => m.Movie)
                          where per.Actor.Name == "Samuel L. Jackson"
                          select new
                          {
                              per.Character,
                              per.Movie.Title,
                              per.Movie.Director
                          };

            foreach (var res in querry1)
            {
                Console.WriteLine("\t {0} \t {1} \t {2}", res.Character, res.Title, res.Director);
            };

            Console.WriteLine();
            Console.WriteLine("2. Mostrar o nome e idade de todos atores que desempenharam um determinado personagem(por exemplo, quais os atores que já atuaram como '007' ?");

            var querry2 = from ator in _db.Characters
                                              .Include(a => a.Actor)
                          where ator.Character == "James Bond"
                          select new
                          {
                              ator.Actor.Name,
                              ator.Actor.DateBirth
                          };

            foreach (var res in querry2)
            {
                int idade = DateTime.Now.Year - res.DateBirth.Year;
                if (DateTime.Now.DayOfYear < res.DateBirth.DayOfYear)
                {
                    idade = idade - 1;
                }

                Console.WriteLine("\t {0} \t {1}", res.Name, idade);
            };

            Console.WriteLine();
            Console.WriteLine("3. Informar qual o ator desempenhou mais vezes um determinado personagem(por exemplo: qual o ator que realizou mais filmes como o 'agente 007'");

            var querry3 = from ator2 in _db.Characters
                                               .Include(a => a.Actor)
                                               .Include(m => m.Movie)
                          where ator2.Character == "James Bond"
                          select new
                          {
                              ator2.Character,
                              ator2.Actor.Name,
                              ator2.Movie.Title
                          };

            int numVezes    = 0;
            String nomeAtor = "";

            foreach (var res in querry3)
            {
                int cont = 1;

                foreach (var res2 in querry3)
                {
                    if (res.Title != res2.Title)
                    {
                        cont = cont + 1;
                    };
                };

                if (cont > numVezes)
                {
                    numVezes = cont;
                    nomeAtor = res.Name;
                };
            };

            Console.WriteLine("\t {0}", nomeAtor);

            Console.WriteLine();
            Console.WriteLine("4. Mostrar o nome e a data de nascimento do ator mais idoso");

            var querry4 = from ator3 in _db.Actors
                          select new
                          {
                              ator3.Name,
                              ator3.DateBirth
                          };

            nomeAtor = "";
            DateTime dataNascimento = DateTime.Now;

            foreach (var res in querry4)
            {
                if (res.DateBirth < dataNascimento)
                {
                    dataNascimento = res.DateBirth;
                    nomeAtor = res.Name;
                };
            };

            Console.WriteLine("\t {0} \t {1}", nomeAtor, dataNascimento);

            Console.WriteLine();
            Console.WriteLine("5. Mostrar o nome e a data de nascimento do ator mais novo a atuar em um determinado gênero");

            var querry5 = from ator4 in _db.Characters
                                               .Include(a => a.Actor)
                                               .Include(m => m.Movie)
                                               .ThenInclude(g => g.Genre)
                          where ator4.Movie.Genre.Name == "Action"
                          select new
                          {
                              ator4.Actor.Name,
                              ator4.Actor.DateBirth
                          };

            nomeAtor = "";
            dataNascimento = DateTime.Now;
            int contadorAux = 0;

            foreach (var res in querry5)
            {
                if (res.DateBirth > dataNascimento||contadorAux == 0)
                {
                    dataNascimento = res.DateBirth;
                    nomeAtor = res.Name;
                    contadorAux = contadorAux + 1;
                };
            };

            Console.WriteLine("\t {0} \t {1}", nomeAtor, dataNascimento);

            Console.WriteLine();
            Console.WriteLine("6. Mostrar o valor médio das avaliações dos filmes de um determinado diretor");

            var querry6 = from val in _db.Movies
                          where val.Director == "Roger Spottiswoode"
                          select new
                          {
                              val.Title,
                              val.Rating
                          };

            contadorAux = 0;
            double medAvaliacoes = 0;

            foreach (var res in querry6)
            {
                contadorAux = contadorAux + 1;
                medAvaliacoes = medAvaliacoes + res.Rating;
            };

            medAvaliacoes = medAvaliacoes / contadorAux;

            Console.WriteLine("\t {0}", medAvaliacoes);

            Console.WriteLine();
            Console.WriteLine("7. Qual o elenco do filme melhor avaliado ?");

            var querry7 = from elenco in _db.Movies
                          select new
                          {
                              elenco.MovieId,
                              elenco.Rating
                          };

            medAvaliacoes = 0;
            int melhorFilme = 0;

            foreach (var res in querry7)
            {
                if (res.Rating > medAvaliacoes)
                {
                    medAvaliacoes = res.Rating;
                    melhorFilme = res.MovieId;
                };
            };

            var querry7b = from lista in _db.Characters
                                                .Include(m => m.Movie)
                                                .Include(a => a.Actor)
                           where lista.MovieId == melhorFilme
                           select new
                           {
                               lista.Actor.Name
                           };

            foreach (var res in querry7b)
            {
                Console.WriteLine("\t {0}", res.Name);
            };

            Console.WriteLine();
            Console.WriteLine("8. Qual o elenco do filme com o maior faturamento?");

            var querry8 = from elenco2 in _db.Movies
                          select new
                          {
                              elenco2.MovieId,
                              elenco2.Gross
                          };

            melhorFilme = 0;
            decimal faturamento = 0;

            foreach (var res in querry8)
            {
                if (res.Gross > faturamento)
                {
                    faturamento = res.Gross;
                    melhorFilme = res.MovieId;
                };
            };

            var querry8b = from lista2 in _db.Characters
                                                .Include(m => m.Movie)
                                                .Include(a => a.Actor)
                           where lista2.MovieId == melhorFilme
                           select new
                           {
                               lista2.Actor.Name
                           };

            foreach (var res in querry8b)
            {
                Console.WriteLine("\t {0}", res.Name);
            };

            Console.WriteLine();
            Console.WriteLine("9. Gerar um relatório de aniversariantes, agrupando os atores pelo mês de aniverário.");

            var querry9 = (from aniver in _db.Actors
                           select new
                           {
                               aniver.Name,
                               aniver.DateBirth,
                               MesAniver = aniver.DateBirth.Month
                           }).ToList();

            var querry9b = from e in querry9
                           group e by e.MesAniver into grp
                           select new
                           {
                               Mes = grp.Key,
                               Aniversariantes = grp
                           };

            foreach (var entrada in querry9b)
            {
                Console.WriteLine("Mes : " + entrada.Mes);

                foreach (var elem in entrada.Aniversariantes)
                {
                    Console.WriteLine("{\t{1} {0}" ,elem.Name, elem.DateBirth);

                }

            }

            Console.WriteLine("- - -   feito!  - - - ");
            Console.WriteLine();
        }

        static void Main_presencial(String[] args)
        {
            // acesso ao EF serah realizado pela variavel _db
            // essa dependencia da camada de apresentacao com
            // a camada de persistencia eh errada!
            MovieContext _db = new MovieContext();

            #region # LINQ - consultas

            Console.WriteLine();
            Console.WriteLine("1. Todos os filmes de acao");

            Console.WriteLine("1a. Modelo tradicional");
            List<Movie> filmes1a = new List<Movie>();
            foreach (Movie f in _db.Movies.Include("Genre"))
            {
                if (f.Genre.Name == "Action")
                    filmes1a.Add(f);
            }

            foreach (Movie filme in filmes1a)
            {
                Console.WriteLine("\t{0} - {1}", filme.Title, filme.ReleaseDate.Year);
            }

            Console.WriteLine("\n1b. Usando linq - query syntax");
            var filmes1b = from f in _db.Movies
                          where f.Genre.Name == "Action"
                          select f;
            foreach (Movie filme in filmes1b)
            {
                Console.WriteLine("\t{0} - {1}", filme.Title, filme.Director);
            }

            Console.WriteLine("\n1c. Usando linq - method syntax");
            var filmes1c = _db.Movies.Where(m => m.Genre.Name == "Action");
            foreach (Movie filme in filmes1c)
            {
                Console.WriteLine("\t{0}", filme.Title);
            }

 
            Console.WriteLine();
            Console.WriteLine("2. Todos os diretores de filmes do genero 'Action', com projecao");
            var filmes2 = from f in _db.Movies
                          where f.Genre.Name == "Action"
                          select f.Director;

            foreach (var nome in filmes2)
            {
                Console.WriteLine("\t{0}", nome);
            }

            Console.WriteLine();
            Console.WriteLine("3a. Todos os filmes de cada genero (query syntax):");
            var generosFilmes3a = from g in _db.Genres.Include(gen => gen.Movies)
                                select g;
            foreach (var gf in generosFilmes3a)
            {
                if (gf.Movies.Count > 0)
                {
                    Console.WriteLine("\nFilmes do genero: " + gf.Name);
                    foreach (var f in gf.Movies)
                    {
                        Console.WriteLine("\t{0} - {1}", f.Title, f.Rating);
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine("3b. Todos os filmes de cada genero (method syntax):");

            var generosFilmes3b = _db.Genres.Include(gen => gen.Movies).ToList();

            foreach (Genre gf in generosFilmes3a)
            {
                if (gf.Movies.Count > 0)
                {
                    Console.WriteLine("\nFilmes do genero: " + gf.Name);
                    foreach (var f in gf.Movies)
                    {
                        Console.WriteLine("\t{0}", f.Title);
                    }
                }
            }


            Console.WriteLine();
            Console.WriteLine("4. Titulo e ano dos filmes do diretor Quentin Tarantino, com projcao em uma class anonima:");
            var tarantino = from f in _db.Movies
                            where f.Director == "Quentin Tarantino"
                             select new
                             {
                                 Ano = f.ReleaseDate.Year,
                                 f.Title
                             };

            foreach (var item in tarantino)
            {
                Console.WriteLine("{0} - {1}", item.Ano, item.Title);
            }

            Console.WriteLine();
            Console.WriteLine("5. Todos os gêneros ordenados pelo nome:");
            var q5 = _db.Genres.OrderByDescending(g => g.Name);
            foreach (var genero in q5)
            {
                Console.WriteLine("{0, 20}\t {1}", genero.Name, genero.Description.Substring(0, 30));
            }

            Console.WriteLine();
            Console.WriteLine("6. Numero de filmes agrupados pelo anos de lançamento:");
            var q6 = from f in _db.Movies
                     group f by f.ReleaseDate.Year into grupo
                     select new
                     {
                         Chave = grupo.Key,
                         NroFilmes = grupo.Count()
                     };

            foreach (var ano in q6.OrderByDescending(g => g.NroFilmes))
            {
                Console.WriteLine("Ano: {0}  Numero de filmes: {1}", ano.Chave, ano.NroFilmes);

            }

            Console.WriteLine();
            Console.WriteLine("7. Projeção do faturamento total, quantidade de filmes e avaliação média agrupadas por gênero:");
            var q7 = from f in _db.Movies
                     group f by f.Genre.Name into grpGen
                     select new
                     {
                         Categoria = grpGen.Key,
                         Faturamento = grpGen.Sum(e => e.Gross),
                         Avaliacao = grpGen.Average(e => e.Rating),
                         Quantidade = grpGen.Count()
                     };

            foreach (var genero in q7)
            {
                Console.WriteLine("Genero: {0}", genero.Categoria);
                Console.WriteLine("\tFaturamento total: {0}\n\t Avaliação média: {1}\n\tNumero de filmes: {2}",
                                genero.Faturamento, genero.Avaliacao, genero.Quantidade);
            }
            #endregion



        }

        static void Main_CRUd(string[] args)
        {
            Console.WriteLine("Hello World!");

            MovieContext _context = new MovieContext();

            Genre g1 = new Genre()
            {
                Name = "Comedia",
                Description = "Filmes de comedia"
            };

            Genre g2 = new Genre()
            {
                Name = "Ficcao",
                Description = "Filmes de ficcao"
            };

            _context.Genres.Add(g1);
            _context.Genres.Add(g2);

            _context.SaveChanges();

            List<Genre> genres = _context.Genres.ToList();

            foreach (Genre g in genres)
            {
                Console.WriteLine(String.Format("{0,2} {1,-10} {2}",
                                    g.GenreId, g.Name, g.Description));
            }

        }
    }
}
