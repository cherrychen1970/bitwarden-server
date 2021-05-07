namespace Bit.Core.Models
{
   public class TaxRate: IKey<string>
   {
      public string Id { get; set; }
      public string Country { get; set; }
      public string State { get; set; }
      public string PostalCode { get; set; }
      public decimal Rate { get; set; }
      public bool Active { get; set; } 

      public void SetNewId()
      {
         // Id is created by Stripe, should exist before this gets called
         return;
      }
   }
}
