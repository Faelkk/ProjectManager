# Project Manager

O **Project Manager** é uma aplicação full stack para controle, gerenciamento e edição de projetos, com autenticação via **JWT**. Utiliza **GraphQL** com **MongoDB** para uma estrutura flexível de dados, e segue a **Clean Architecture** para garantir escalabilidade, testabilidade e manutenibilidade.

## 🏛️ Arquitetura

O projeto adota a Clean Architecture, promovendo uma separação clara entre as camadas de domínio, aplicação, infraestrutura e apresentação.

Para garantir facilidade de deploy e ambiente padronizado, o projeto está containerizado utilizando **Docker**.

## 🗂️ Padrões e Conceitos Utilizados

- **Camada de Domínio**  
  Contém as entidades e regras de negócio puras, independentes de tecnologias externas.

- **Camada de Aplicação**  
  Coordena os casos de uso, consumindo repositórios e serviços externos.

- **Camada de Infraestrutura**  
  Implementações técnicas como repositórios, acesso a banco, APIs externas, etc.

- **Camada de Apresentação / Interface**  
  Expõe a API via GraphQL. Consome os serviços da aplicação e utiliza DTOs para comunicação.

- **DTO (Data Transfer Object)**  
  Utilizados para transportar dados entre as camadas, de forma segura e eficiente.

- **Dependency Injection (Injeção de Dependências)**  
  Reduz acoplamento entre os componentes e facilita os testes.

## 🛠️ Tecnologias Utilizadas

- [.NET](https://dotnet.microsoft.com/pt-br/)
- [HotChocolate (GraphQL)](https://learn.microsoft.com/pt-br/shows/on-dotnet/getting-started-with-hotchocolate)
- [Authentication.JwtBearer](https://learn.microsoft.com/pt-br/dotnet/api/microsoft.aspnetcore.authentication.jwtbearer?view=aspnetcore-9.0)
- [HotChocolate.AspNetCore.Authorization](https://www.nuget.org/packages/HotChocolate.AspNetCore.Authorization/)
- [MongoDB + MongoDB.Driver](https://www.mongodb.com/docs/drivers/csharp/current/)

## Testes

- [xUnit](https://learn.microsoft.com/pt-br/dotnet/core/testing/unit-testing-with-dotnet-test)
- [Moq](https://github.com/moq/moq4)
- [InMemory Provider (Mongo2Go)](https://github.com/Mongo2Go/Mongo2Go)

## DevOps e Containers

- [Docker](https://www.docker.com/)
- [Docker Compose](https://docs.docker.com/compose/)
- [Git](https://git-scm.com/)

---

🧪 **Como Rodar os Testes**

O projeto possui testes automatizados para garantir a qualidade do código e o funcionamento correto das funcionalidades implementadas. Veja abaixo como executar esses testes:

✅ Executar Testes Unitários

Abra o terminal na pasta ProjectManager.Test do projeto e execute o comando:

    $ dotnet test

🧩 Executar Testes de Integração

Abra o terminal na pasta ProjectManager.Test.Test do projeto e Execute o comando:

    $ dotnet test

🚀 **Como Rodar o Projeto**

Para rodar o projeto em seu ambiente local, siga os passos abaixo:

1.  Clonar o Repositório
    Primeiramente, clone o repositório do GitHub para sua máquina local:

        $ git clone https://github.com/Faelkk/ProjectManager

2.  Instalar as Dependências
    Acesse o diretório do projeto e instale as dependências:

        $ dotnet restore

3.  Configurar as variaveis de ambiente

        $ { "DatabaseSettings": {"ConnectionString": "urlConnection","DatabaseName": "databasename"},"JwtSettings": {"Secret": "exemplodesecret","ExpiresDay": 7}}

4.  Configurar o Docker Compose
    Antes de rodar o projeto, configure as variáveis do docker-compose de acordo com as suas necessidades. Certifique-se de que o Docker e o Docker Compose estão instalados corretamente em sua máquina.

5.  Construir o Projeto com Docker
    Crie as imagens do Docker para o projeto:

        $ docker compose build

6.  Subir o Projeto
    Finalmente, suba o projeto utilizando Docker Compose:

        $ docker compose up -d

<br>

🤝 **Como Contribuir?**

- ⭐ Deixe uma estrela no repositório.
- 🔗 Me siga aqui no GitHub.
- 👥 Conecte-se comigo no LinkedIn e faça parte da minha rede profissional.

👨‍💻**Autor**
Desenvolvido por [Rafael Achtenberg](linkedin.com/in/rafael-achtenberg-7a4b12284/).
