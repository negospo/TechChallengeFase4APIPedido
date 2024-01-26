Feature: Gerenciamento de Clientes

Scenario: Obter detalhes de um cliente
	Given Eu tenho um ID de cliente existente
	When Eu solicito os detalhes do cliente com o ID fornecido
	Then Os detalhes do cliente são retornados com sucesso

Scenario: Tentar obter detalhes de um cliente inexistente
	Given Eu tenho um ID de cliente inexistente
	When Eu tento obter os detalhes do cliente com o ID fornecido
	Then Uma exceção de Cliente não encontrado é lançada

Scenario: Obter detalhes de um cliente pelo CPF
	Given Eu tenho um CPF de cliente existente
	When Eu solicito os detalhes do cliente com o CPF fornecido
	Then Os detalhes do cliente são retornados com sucesso por meio do CPF

Scenario: Tentar obter detalhes de um cliente pelo CPF inexistente
	Given Eu tenho um CPF de cliente inexistente
	When Eu tento obter os detalhes do cliente com o CPF fornecido
	Then Uma exceção de Cliente não encontrado é lançada

Scenario: Listar todos os clientes
	Given Existem clientes registrados
	When Eu solicito a lista de todos os clientes
	Then A lista de clientes é retornada com sucesso

Scenario: Excluir um cliente
	Given Eu tenho um ID de cliente existente
	When Eu solicito a exclusão do cliente com o ID fornecido
	Then O cliente é excluído com sucesso

Scenario: Tentar excluir um cliente inexistente
	Given Eu tenho um ID de cliente inexistente
	When Eu tento excluir o cliente com o ID fornecido
	Then Uma exceção de Cliente não encontrado é lançada

Scenario: Inserir um novo cliente
	Given Eu tenho informações de um novo cliente
	When Eu solicito a inserção do novo cliente
	Then O cliente é inserido com sucesso

Scenario: Tentar inserir um cliente com CPF duplicado
	Given Eu tenho informações de um cliente com CPF já existente
	When Eu tento inserir o cliente duplicado
	Then Uma exceção de Conflito é lançada indicando CPF duplicado

Scenario: Tentar inserir um cliente com email duplicado
	Given Eu tenho informações de um cliente com email já existente
	When Eu tento inserir o cliente duplicado
	Then Uma exceção de Conflito é lançada indicando email duplicado

Scenario: Atualizar os detalhes de um cliente
	Given Eu tenho informações de atualização de um cliente existente
	When Eu solicito a atualização do cliente com as novas informações
	Then Os detalhes do cliente são atualizados com sucesso

Scenario: Tentar atualizar um cliente sem ID
	Given Eu tenho informações de atualização de cliente sem ID
	When Eu tento atualizar o cliente sem ID
	Then Uma exceção de Conflito é lançada indicando que o ID do cliente está vazio

Scenario: Tentar atualizar um cliente com CPF duplicado
	Given Eu tenho informações de atualização de um cliente existente com CPF duplicado
	When Eu tento atualizar um cliente com email ou CPF duplicado
	Then Uma exceção de Conflito é lançada indicando CPF duplicado

Scenario: Tentar atualizar um cliente com email duplicado
	Given Eu tenho informações de atualização de um cliente existente com email duplicado
	When Eu tento atualizar um cliente com email ou CPF duplicado
	Then Uma exceção de Conflito é lançada indicando email duplicado