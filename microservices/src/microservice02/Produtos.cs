using Bogus;

namespace Service_02
{
    public class Produtos
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public int Quantidade { get; set; }
        public decimal Valor { get; set; }

        public List<Produtos> CriarListaProduto(int total)
        {
            var faker = new Faker("pt_BR");

            var produtos = new List<Produtos>();

            for (int i = 0; i < total; i++)
            {
                var produto = new Produtos
                {
                    Id = Guid.NewGuid(),
                    Nome = faker.Commerce.ProductName(),
                    Quantidade = faker.Random.Number(1, 100),
                    Valor = faker.Random.Decimal(1, 1000)
                };

                produtos.Add(produto);
            }

            return produtos;
        }
    }
}
