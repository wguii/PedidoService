

### Propósito do Sistema

Este sistema tem como objetivo manejar o sistema de vendas de uma loja de eletrônicos fictícia, auxiliando no controle de estoque e registro de pedidos e clientes. 

A aplicação foi desenvolvida utilizando a arquitetura de microsserviços para proporcionar maior escalabilidade, modularidade e facilidade de manutenção e usando as tecnologias .NET.

Este projeto foi realizado para o trabalho final da disciplina de Arquitetura de Software da UNISATC.

---

### Usuário do Sistema

- **Administradores**: responsáveis por cadastrar, atualizar e remover produtos, consultar pedidos e acompanhar o estoque.
  
---

### Requisitos Funcionais

- RF001. O sistema deve permitir o cadastro de novos produtos.
- RF002. O sistema deve permitir a atualização de informações de produtos existentes.
- RF003. O sistema deve permitir a consulta de produtos disponíveis.
- RF004. O sistema deve permitir a realização de pedidos de compra.
- RF005. O sistema deve permitir consultar informações de pedidos.
- RF006. O sistema deve permitir cadastrar e gerenciar clientes novos.
- RF006. Os microsserviços devem se comunicar via API REST.
- RF007. Os dados devem ser armazenados em um banco de dados relacional SQLite, sendo um banco exclusivo para cada microsserviço.

---

## Descritivo Técnico

### Microsserviços

- **ProdutoService**
  - Responsável pela criação e gestão dos produtos.

- **PedidoService**
  - Responsável pelo cadastro e gerenciamento de pedidos realizados pelos clientes.
  - Valida nos outros microsserviços a existência do cliente e do produto especificado no pedido.
  - Verifica se a quantidade especificada no pedido é menor do que o estoque disponível.

- **[ClienteService]** (se houver)
  - Responsável pelo cadastro e gerenciamento dos clientes da loja.


### Tecnologias Utilizadas

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- Swagger (para documentação de APIs)

---

## Integrantes da Equipe

- Guilherme Zapelini
- Alexandre Sartor
- Davi Carlos

---

### Como rodar o projeto

- Clone os três repositórios correspondentes aos três microsserviços necessários para a aplicação:

- **ProdutoService: https://github.com/wguii/ProdutoService.git (rodando na porta :5001)**
- **ClienteService: https://github.com/wguii/ClienteService.git (rodando na porta :5002)**
- **PedidoService: https://github.com/wguii/PedidoService.git (rodando na porta :5003)**

-Restaure as dependências com "dot net restore"

-Aplique as migrations com "dotnet ef database update"

-Rode as três aplicações e faça requisições aos endpoints no Swagger

