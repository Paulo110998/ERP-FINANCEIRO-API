# ERP Financeiro - API REST

## Descrição

Este projeto é uma API REST para um ERP Financeiro, desenvolvido com ASP.NET Core e C#. A API oferece funcionalidades para controle de usuários, contas a pagar, pagos, categorias, beneficiários, exportação de PDFs e todo o controle financeiro em um só lugar.

## Tecnologias Utilizadas

- **Linguagem**: C#
- **Framework**: .NET 8, ASP.NET Core
- **ORM**: Entity Framework Core e Fluent API
- **Consultas**: LINQ
- **Autenticação**: Identity e JWT
- **Arquitetura**: Injeção de dependências, Princípios SOLID
- **Geração de PDF**: QuestPDF
- **Banco de Dados**: MySQL

## Estrutura do Projeto

A estrutura do projeto está organizada da seguinte forma:

- **Authorization**: Contém a lógica de autorização de acesso.
- **Controllers**: Contém um controlador para cada entidade.
- **Data**
  - **DTO**: Contém os Data Transfer Objects para cada tipo de operação (CreateDto, ReadDto, UpdateDto).
  - **Context**: Contém os arquivos de contexto de banco de dados.
- **Models**: Contém os modelos de dados de cada entidade.
- **Profiles**: Contém os Profiles de mapeamento com AutoMapper para injeção nos controllers e services.
- **Services**: Contém a lógica de negócio para cada entidade. Os controladores executam métodos dos serviços sem lógica de negócio.
- **Program.cs**: Arquivo de configuração onde são injetados os services e a lógica de inicialização.

## Funcionalidades

- **Controle de Usuários**: CRUD de usuários com autenticação via Identity e JWT.
- **Contas a Pagar e Pagas**: Gerenciamento completo das contas financeiras.
- **Categorias**: Gestão de categorias para classificação de contas.
- **Beneficiários**: Gerenciamento de beneficiários para pagamentos.
- **Exportação de PDFs**: Geração de relatórios financeiros em PDF utilizando QuestPDF.
