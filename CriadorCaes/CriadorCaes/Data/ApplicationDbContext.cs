using CriadorCaes.Models;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CriadorCaes.Data {

   /// <summary>
   /// Esta classe representa a BD do projeto 
   /// </summary>
   public class ApplicationDbContext : IdentityDbContext {

      public ApplicationDbContext(
         DbContextOptions<ApplicationDbContext> options)
          : base(options) {
      }

      /*
       * Criação das tabelas da Base de Dados
       */

      public DbSet<Criadores> Criadores { get; set; }
      public DbSet<Racas> Racas { get; set; }
      public DbSet<Animais> Animais { get; set; }
      public DbSet<Fotografias> Fotografias { get; set; }


   }
}