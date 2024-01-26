Feature: Gerenciamento de Produtos

Scenario: Obter um produto por ID
	Given Existe um produto com o ID especificado
	When Eu solicito o produto com o ID específico
	Then O produto correspondente é retornado

Scenario: Tentar obter detalhes de um produto inexistente
	Given Eu tenho um ID de produto inexistente
	When Eu tento obter os detalhes do produto com o ID fornecido
	Then Uma exceção de Produto não encontrado é lançada

Scenario: Listar todos os produtos
	Given Existem produtos cadastrados no sistema
	When Eu solicito a lista de todos os produtos
	Then Todos os produtos são retornados

Scenario: Listar produtos por categoria
	Given Existem produtos cadastrados no sistema
	And Cada produto possui uma categoria atribuída
	When Eu solicito a lista de produtos de uma categoria específica
	Then Apenas os produtos da categoria especificada são retornados

Scenario: Excluir um produto por ID
	Given Existe um produto com o ID especificado
	When Eu solicito a exclusão do produto com o ID específico
	Then O produto é removido do sistema

Scenario: Tentar excluir um produto inexistente
	Given Eu tenho um ID de produto inexistente
	When Eu tento excluir o produto com o ID fornecido
	Then Uma exceção de Produto não encontrado é lançada

Scenario: Inserir um novo produto
	Given Eu tenho informações válidas para um novo produto
	When Eu solicito a inserção do novo produto
	Then O novo produto é cadastrado no sistema

Scenario: Atualizar um produto existente
	Given Existe um produto com o ID especificado
	And Eu tenho informações atualizadas para o produto
	When Eu solicito a atualização do produto com o ID especifico
	Then O produto é atualizado com as novas informações

Scenario: Tentar atualizar um produto sem ID
	Given Eu tenho informações de atualização de produto sem ID
	When Eu tento atualizar o produto
	Then Uma exceção de Conflito é lançada indicando que o ID está vazio

Scenario: Tentar atualizar um produto inexistente
	Given Eu tenho um ID de produto inexistente e informações de atualização
	When Eu tento atualizar o produto com as novas informações
	Then Uma exceção de Produto não encontrado é lançada