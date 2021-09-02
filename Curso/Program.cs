using CursoEfCore.Domain;
using CursoEfCore.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CursoEfCore
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //  InserirDadosEmMassa();
            ConsultarDados();

            //     using var ctx = new Data.ApplicationContext();
            //    Console.WriteLine(ctx.Database.GetPendingMigrations().Count());
        }

        private static void InserirDados()
        {
            var produto = new Produto
            {
                Descricao = "Caneta Bic",
                Ativo = true,
                CodigoBarras = "1234567890",
                TipoProduto = ValueObjects.TipoProduto.MercadoriaParaRevenda,
                Valor = 10m
            };

            using var db = new Data.ApplicationContext();

            db.Produtos.Add(produto);
            // db.Set<Produto>().Add(produto);
            // db.Entry(produto).State = EntityState.Added;
            // db.Add(produto);

            var registros = db.SaveChanges();
            System.Console.WriteLine("Total registro(s): {0}", registros);
        }

        private static void InserirDadosEmMassa()
        {
            var produto = new Produto
            {
                Descricao = "Caneta Bic",
                Ativo = true,
                CodigoBarras = "1234567890",
                TipoProduto = ValueObjects.TipoProduto.MercadoriaParaRevenda,
                Valor = 10m
            };

            var cliente = new Cliente
            {
                Nome = "Maria",
                Cidade = "Curitiba",
                CEP = "99999",
                Estado = "PR",
                Telefone = "4199886655"
            };

            using var db = new Data.ApplicationContext();
            db.AddRange(produto, cliente);

            var listaClientes = new[]{
                new Cliente
                {
                    Nome = "Maria",
                    Cidade = "Curitiba",
                    CEP = "99999",
                    Estado = "PR",
                    Telefone = "4199886655"
                },
                new Cliente
                {
                    Nome = "Joao",
                    Cidade = "Curitiba",
                    CEP = "99999",
                    Estado = "PR",
                    Telefone = "4199889988"
                }
            };

            db.AddRange(listaClientes);

            var registros = db.SaveChanges();
            System.Console.WriteLine("Total registro(s): {0}", registros);
        }

        private static void ConsultarDados()
        {
            using var db = new Data.ApplicationContext();

            var consulta = db.Clientes.Where(c => c.Id >= 0)
            .OrderBy(o => o.Nome)
            .ToList();

            foreach (var cliente in consulta)
            {
                System.Console.WriteLine($"Cliente {cliente.Id}: {cliente.Nome}");
            }
        }

        private static void ConsultarPedidoCarregamentoAdiantado()
        {
            using var db = new Data.ApplicationContext();
            var pedidos = db
                .Pedidos
                .Include(p => p.Itens)
                    .ThenInclude(p => p.Produto)
                .ToList();

            Console.WriteLine(pedidos.Count);
        }

        private static void CadastrarPedido()
        {
            using var db = new Data.ApplicationContext();

            var cliente = db.Clientes.FirstOrDefault();
            var produto = db.Produtos.FirstOrDefault();

            var pedido = new Pedido
            {
                ClienteId = cliente.Id,
                IniciadoEm = DateTime.Now,
                FinalizadoEm = DateTime.Now,
                Observacao = "Pedido Teste",
                Status = StatusPedido.Analise,
                TipoFrete = TipoFrete.SemFrete,
                Itens = new List<PedidoItem>
                 {
                     new PedidoItem
                     {
                         ProdutoId = produto.Id,
                         Desconto = 0,
                         Quantidade = 1,
                         Valor = 10,
                     }
                 }
            };

            db.Pedidos.Add(pedido);

            db.SaveChanges();
        }

        private static void AtualizarDados()
        {
            using var db = new Data.ApplicationContext();
            //var cliente = db.Clientes.Find(1);

            var cliente = new Cliente
            {
                Id = 1
            };

            var clienteDesconectado = new
            {
                Nome = "Cliente Desconectado Passo 3",
                Telefone = "7966669999"
            };

            db.Attach(cliente);
            db.Entry(cliente).CurrentValues.SetValues(clienteDesconectado);

            //db.Clientes.Update(cliente);
            db.SaveChanges();
        }

        private static void RemoverRegistro()
        {
            using var db = new Data.ApplicationContext();

            //var cliente = db.Clientes.Find(2);
            var cliente = new Cliente { Id = 3 };
            //db.Clientes.Remove(cliente);
            //db.Remove(cliente);
            db.Entry(cliente).State = EntityState.Deleted;

            db.SaveChanges();
        }
    }
}