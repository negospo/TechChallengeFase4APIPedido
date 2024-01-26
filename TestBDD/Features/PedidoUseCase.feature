Feature: Gerenciamento de Pedidos de Pagamento

Scenario: Obter um pedido pelo ID
	Given Que exista um pedido com o ID específico
	When Eu solicito o pedido pelo ID
	Then Devo receber o pedido correspondente

Scenario: Listar todos os pedidos
	Given Que existem pedidos no sistema
	When Eu solicito a lista de todos os pedidos
	Then Devo receber uma lista contendo todos os pedidos

Scenario: Listar pedidos por status
	Given Que existem pedidos com diferentes status
	When Eu solicito a lista de pedidos filtrados por um status específico
	Then Devo receber uma lista contendo apenas os pedidos com o status especificado

Scenario: Criar um novo pedido
	Given Que tenho os detalhes de um novo pedido
	When Eu crio um novo pedido
	Then Deve ser criado um novo pedido com sucesso

Scenario: Busca o status de um pagamento
	Given Que existe um pedido com um status específico
	When Eu solicito o status desse pedido
	Then Devo receber o status do pedido correspondente

Scenario: Atualizar o status de um pedido
	Given Que existe um pedido com um status específico
	When Eu atualizo o status desse pedido
	Then O status do pedido deve ser atualizado com sucesso

