# 🎮 VendaPass — Automação de Vendas de Passes/Moedas de Jogos

## 📌 Descrição
O **VendaPass** é um sistema web criado para automatizar a venda de passes/moedas de jogos, resolvendo um problema real: **um vendedor que gerenciava todo o processo manualmente via Discord e planilhas**.  
Com o VendaPass, o fluxo se torna organizado, seguro e escalável, trazendo agilidade para clientes e controle total para administradores.

## 🔗 Demonstração
[Em breve: link da demo](#)

## ⚙️ Funcionalidades
- **Cadastro e login com Identity** (usuários e roles)
- **Confirmação de e-mail**
- **Sistema de compras integrado**
- **Dashboard do cliente**
- **Dashboard do administrador**
- **Filtro avançado de dados**
- **Gestão de pedidos e usuários**
- Interface **moderna/gamer**

## 🛠️ Tecnologias Utilizadas
- **ASP.NET Core MVC**
- **C#**
- **Entity Framework**
- **SQL Server**
- **ASP.NET Identity**
- HTML, CSS e JavaScript

## 🏗️ Arquitetura (resumo)
O projeto segue o padrão **MVC (Model-View-Controller)**, com camadas bem definidas:
- **Controllers**: regras de negócio e fluxo do sistema  
- **Models**: entidades e acesso aos dados  
- **Views**: interface do usuário  

## 🔒 Segurança
- Autenticação e autorização via **ASP.NET Identity**
- Controle de acesso por **roles (admin/cliente)**
- Confirmação de e-mail para validação de conta

## ▶️ Como rodar o projeto
1. Clone o repositório  
   ```bash
   git clone https://github.com/GuilhermeOliveira-Silva/ProjetoVendaPass
   ```
2. Configure a string de conexão no `appsettings.json`
3. Execute as migrations  
   ```bash
   dotnet ef database update
   ```
4. Inicie o projeto  
   ```bash
   dotnet run
   ```

## 🚀 Melhorias Futuras
- Integração completa com **Discord**
- Área de notificações em tempo real
- Painel de relatórios avançados

## 👤 Autor
**Guilherme Oliveira**  
[GitHub](https://github.com/GuilhermeOliveira-Silva)
