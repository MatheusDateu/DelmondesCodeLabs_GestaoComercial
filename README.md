# Delmondes CodeLabs - Gestão Comercial & PDV

<p align="center">
  <b>Sistema de Alta Performance para Gerenciamento Comercial, PDV e Controle de Estoque</b><br>
  <i>Desenvolvido por Delmondes CodeLabs</i>
</p>

---

## 1. Visão Geral do Sistema
O **Delmondes CodeLabs - Gestão Comercial & PDV** é um sistema de alta performance modelado inicialmente para o nicho de adegas, lojas de conveniência e comércios correlatos. A plataforma oferece controle robusto de estoque, composições de serviços (como sessões de narguilé com dedução automática de insumos), controle de despesas operacionais e um Ponto de Venda (PDV) ágil com suporte a comandas abertas.

---

## 2. Arquitetura e Stack Tecnológica
* **Backend:** C# / .NET 10.0.
* **Frontend:** Blazor Web App (Interactive Server) + MudBlazor (Mobile-First UI em Material Design estrito).
* **Banco de Dados:** PostgreSQL orquestrado via Docker.
* **Padrão de Acesso a Dados (CQRS):**
  * **Commands (Escritas):** Entity Framework Core (v10.0.10) estritamente para inserções, atualizações e exclusões, garantindo integridade transacional.
  * **Queries (Leituras):** Dapper (v2.1.79) estritamente para consultas de alta performance utilizando SQL puro, evitando sobrecarga de memória.

---

## 3. Principais Módulos e Regras de Negócio
* **Catálogo de Produtos:** Gerenciamento de categorias, SKUs únicos, códigos de barra, preços de custo/venda e alertas de estoque mínimo.
* **Motor de Estoque e Composição (BOM):** Rastreabilidade absoluta por meio de movimentos imutáveis de estoque (`StockMovements`). Venda de serviços (como Narguilé) deduz automaticamente os componentes físicos associados (essência, carvão, alumínio).
* **Módulo Financeiro:** Acompanhamento de despesas fixas e variáveis (aluguel, contas de concessionárias, manutenção) para cálculo preciso do Lucro Líquido no Dashboard de BI.
* **Precificação e Promoções:** Suporte a descontos percentuais e valores fixos vinculados a produtos ou categorias específicas.
* **Ponto de Venda (PDV):** Operação de balcão e comandas abertas. Itens lançados em comandas deduzem o estoque imediatamente; cancelamentos devolvem os itens ao estoque físico, enquanto avarias contabilizam perdas para relatórios gerenciais.
* **Business Intelligence (BI):** Painéis dinâmicos com análise de Curva ABC de vendas, faturamento bruto, CMV, despesas pagas e margem líquida.

---

## 4. Controle de Acesso Baseado em Funções (RBAC)
O sistema implementa quatro níveis hierárquicos de segurança:
* **SuperUser:** Proprietário supremo com acesso total e exclusividade para gerenciar cargos administrativos.
* **Admin:** Gerente de loja com acesso operacional completo (BI, Inventário, Financeiro, PDV, Catálogo e Promoções), sem poder gerenciar SuperUsers.
* **Cashier (Caixa):** Acesso total ao terminal de PDV e histórico de vendas para fechamento de contas.
* **Waiter (Garçom):** Acesso restrito para lançamento de produtos em comandas abertas, sem permissão para recebimento de pagamentos ou exclusão de histórico.

---

## 5. Pré-requisitos para Execução Local
Para rodar o sistema em sua máquina de desenvolvimento, certifique-se de ter os seguintes softwares instalados:
1. **Docker Desktop** (com suporte a containers Linux ativado).
2. **.NET 10.0 SDK** (ou superior).
3. Uma IDE compatível (Visual Studio 2026 / Visual Studio Code / Rider).

---

## 6. Configuração do Ambiente Local
O sistema utiliza variáveis de ambiente injetadas via Docker Compose. Siga os passos abaixo para configurar o seu ambiente:

1. Na raiz do projeto (onde se encontra o arquivo `docker-compose.yml`), crie um arquivo chamado **`.env`**.
2. Cole o seguinte conteúdo de configuração pré-definida no arquivo `.env`:

```env
POSTGRES_USER=root
POSTGRES_PASSWORD=S3nh4.Development@3nv1r0nm3nt
DB_USER=root
DB_PASSWORD=S3nh4.Development@3nv1r0nm3nt
CLOUDFLARE_TOKEN=eyJhIjoiYjM0ZmFiOTFmODY5NmI5ODFlMDEwMjNhNTU2ZDVhMzMiLCJ0IjoiNGYzOTJkYjMtNzQyYi00OWZjLTkwNzAtODk3OTMzZmFkZWY5IiwicyI6IlpXTmxZek5sWlRBdE16SXlZaTAwT0RNeExXRXdNMkl0TldaaFlUUXpOVEEyTUdFeSJ9
NGROK_TOKEN=3HkuvqDAz9mQUnSZB6bBcnDhwlb_7ghwwLTzyA8vmY5rHUXr9
```

---

## 7. Como Executar o Sistema via Linha de Comando (CLI)

Com o Docker Desktop aberto e o arquivo `.env` configurado na raiz, execute os comandos abaixo no seu terminal (PowerShell, Bash ou CMD):

### Para subir a aplicação e o banco de dados em segundo plano:
```bash
docker compose up -d --build
```

### Para acompanhar os logs de execução:
```bash
docker compose logs -f delmondescodelabs_gestaocomercial
```

### Para derrubar os containers e preservar os dados do banco:
```bash
docker compose down
```

### Para derrubar os containers e limpar totalmente os volumes de dados (Reset Geral):
```bash
docker compose down -v
```

Uma vez que os containers estejam rodando, acesse a aplicação no navegador através da porta mapeada ou pelo túnel configurado em:
* `https://localhost:8081` ou `http://localhost:8080`

---

## 8. Licenciamento e Direitos de Uso

Este projeto é distribuído sob os termos da **Apache License 2.0** (consulte o arquivo [LICENSE.txt](LICENSE.txt) para mais detalhes).

* **O que é permitido:** Você tem total liberdade para realizar *forks*, estudar o código-fonte, modificar, adaptar e rodar a aplicação localmente para fins de testes, avaliação, entrevistas ou uso operacional interno.
* **O que é proibido:** É estritamente proibida a **comercialização, revenda, relicenciamento sob marcas de terceiros ou distribuição comercial direta** do software ou de trabalhos derivados pagos sem a autorização expressa da **Delmondes CodeLabs**.
