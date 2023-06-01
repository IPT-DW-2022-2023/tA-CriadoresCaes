using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CriadorCaes.Models {

   /// <summary>
   /// descrição dos cães
   /// </summary>
   public class Animais {

      public Animais() {
         ListaFotografias = new HashSet<Fotografias>();
      }

      public int Id { get; set; }

      /// <summary>
      /// nome do cão
      /// </summary>
      public string Nome { get; set; }

      /// <summary>
      /// data de nascimento do cão/cadela
      /// </summary>
      [Required(ErrorMessage = "a {0} é de preenchimento obrigatório.")]
      public DateTime DataNascimento { get; set; }

      /// <summary>
      /// data de aquisição do cão/cadela
      /// </summary>
      public DateTime? DataCompra { get; set; }
      // o uso do ? torna este atributo de preenchimento facultativo
      // se já tiver sido definida a estrutura da BD
      // é necessário criar uma nova Migration


      /// <summary>
      /// preço de aquisição do cão/cadela
      /// </summary>
      public decimal PrecoCompra { get; set; }

      /// <summary>
      /// atributo auxiliar para ajudar a introdução de dados
      /// sobre o preço de compra do animal
      /// </summary>
      [NotMapped] // esta anotação instrui a Migration para ignorar este atributo
      [RegularExpression("[0-9]+(.|,)?[0-9]{0,2}",
         ErrorMessage ="tem de escrever algarismos, com até dois digitos para a parte fracionária do {0}")]
      public string PrecoCompraAux { get; set; }


      /// <summary>
      /// sexo do animal: 
      /// F - Fêmea
      /// M - Macho
      /// </summary>
      public string Sexo { get; set; }

      /// <summary>
      /// LOP do registo do cão/cadela
      /// </summary>
      public string NumLOP { get; set; }

      /* ++++++++++++++++++++++++++++++++++++++++++ 
       * Criação das chaves forasteiras
       * ++++++++++++++++++++++++++++++++++++++++++ 
       */

      /// <summary>
      /// FK para o Criador do cão/cadela
      /// </summary>
      [ForeignKey(nameof(Criador))]
      [Display(Name = "Criador")] // texto que irá aparecer no ecrã
      public int CriadorFK { get; set; }
      public Criadores Criador { get; set; } // efetivamente, esta é q é a FK, para a EF
      /*
       * o uso de [anotadores] serve para formatar o comportamento
       * dos 'objetos' por ele referenciados
       * estes 'objetos' podem ser:
       *    - atributos
       *    - funções (métodos)
       *    - classes
       * */

      /// <summary>
      /// FK do Animal para a sua Raça
      /// </summary>
      [ForeignKey(nameof(Raca))]
      [Display(Name = "Raça")]
      public int RacaFK { get; set; }
      public Racas Raca { get; set; }

      /// <summary>
      /// Lista das Fotografias associadas ao animal
      /// </summary>
      public ICollection<Fotografias> ListaFotografias { get; set; }



   }
}
