
DO $$
BEGIN 
    IF NOT EXISTS (SELECT FROM pg_tables WHERE schemaname = 'public' AND tablename = 'produto_categoria') THEN 
        CREATE TABLE public.produto_categoria (
            id int4 NOT NULL,
            nome varchar NOT NULL,
            CONSTRAINT produto_categoria_pk PRIMARY KEY (id)
        );

        ALTER TABLE public.produto_categoria OWNER TO postgres;
        GRANT ALL ON TABLE public.produto_categoria TO postgres;
    END IF; 
END $$;



DO $$
BEGIN 
    IF NOT EXISTS (SELECT FROM pg_tables WHERE schemaname = 'public' AND tablename = 'produto') THEN 
        CREATE TABLE public.produto (
            id int4 NOT NULL GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 START 1 CACHE 1 NO CYCLE),
            nome varchar NOT NULL,
            produto_categoria_id int4 NOT NULL,
            preco numeric(10, 2) NOT NULL,
            excluido bool NULL DEFAULT false,
            imagem varchar NULL,
            descricao varchar NULL,
            CONSTRAINT produto_pk PRIMARY KEY (id),
            CONSTRAINT produto_categoria_fk FOREIGN KEY (produto_categoria_id) REFERENCES public.produto_categoria(id)
        );

        ALTER TABLE public.produto OWNER TO postgres;
        GRANT ALL ON TABLE public.produto TO postgres;
    END IF; 
END $$;


DO $$
BEGIN 
    IF NOT EXISTS (SELECT FROM pg_tables WHERE schemaname = 'public' AND tablename = 'cliente') THEN 
        CREATE TABLE public.cliente (
            id int4 NOT NULL GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 START 1 CACHE 1 NO CYCLE),
            nome varchar NOT NULL,
            email varchar NULL,
            cpf varchar NULL,
            excluido bool NULL DEFAULT false,
            CONSTRAINT cliente_pk PRIMARY KEY (id)
        );

        ALTER TABLE public.cliente OWNER TO postgres;
        GRANT ALL ON TABLE public.cliente TO postgres;
    END IF; 
END $$;



DO $$
BEGIN 
    IF NOT EXISTS (SELECT FROM pg_tables WHERE schemaname = 'public' AND tablename = 'pedido') THEN 
        CREATE TABLE public.pedido (
            id int4 NOT NULL GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 START 1 CACHE 1 NO CYCLE),
            "data" timestamp NOT NULL,
            cliente_id int4 NULL,
            anonimo bool NOT NULL,
            anonimo_identificador varchar NULL,         
            valor numeric(10, 2) NOT NULL,
            cliente_observacao varchar NULL,
            CONSTRAINT pedido_pk PRIMARY KEY (id),
            CONSTRAINT cliente_fk FOREIGN KEY (cliente_id) REFERENCES public.cliente(id)          
        );

        ALTER TABLE public.pedido OWNER TO postgres;
        GRANT ALL ON TABLE public.pedido TO postgres;
    END IF; 
END $$;


DO $$
BEGIN 
    IF NOT EXISTS (SELECT FROM pg_tables WHERE schemaname = 'public' AND tablename = 'pedido_item') THEN 
        CREATE TABLE public.pedido_item (
            id int4 NOT NULL GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 START 1 CACHE 1 NO CYCLE),
            pedido_id int4 NOT NULL,
            produto_id int4 NOT NULL,
            preco_unitario numeric(10, 2) NOT NULL,
            quantidade int2 NOT NULL,
            CONSTRAINT pedido_item_pk PRIMARY KEY (id),
            CONSTRAINT pedido_fk FOREIGN KEY (pedido_id) REFERENCES public.pedido(id),
            CONSTRAINT produto_fk FOREIGN KEY (produto_id) REFERENCES public.produto(id)
        );

        ALTER TABLE public.pedido_item OWNER TO postgres;
        GRANT ALL ON TABLE public.pedido_item TO postgres;
    END IF; 
END $$;




INSERT INTO public.produto_categoria (id, nome) VALUES
(1, 'Lanche'),
(2, 'Acompanhamento'),
(3, 'Bebida'),
(4, 'Sobremesa')
ON CONFLICT (id) 
DO NOTHING;



INSERT INTO public.cliente (nome, email, cpf, excluido) 
SELECT 'Pedro Cunha', 'pedro@gmail.com', '07411266051', false
WHERE NOT EXISTS (SELECT 1 FROM public.cliente WHERE cpf = '07411266051');

INSERT INTO public.cliente (nome, email, cpf, excluido) 
SELECT 'João da Silva', 'joao@gmail.com', '66649521060', false
WHERE NOT EXISTS (SELECT 1 FROM public.cliente WHERE cpf = '66649521060');

INSERT INTO public.cliente (nome, email, cpf, excluido) 
SELECT 'Gabriel Santana', 'gaby@gmail.com', '45927804004', false
WHERE NOT EXISTS (SELECT 1 FROM public.cliente WHERE cpf = '45927804004');



INSERT INTO public.produto (nome, produto_categoria_id, preco, excluido, imagem, descricao) 
SELECT 'Big Mac', 1, 9.99, false, NULL, 'Melhor lanche da casa'
WHERE NOT EXISTS (SELECT 1 FROM public.produto WHERE nome = 'Big Mac');

INSERT INTO public.produto (nome, produto_categoria_id, preco, excluido, imagem, descricao) 
SELECT 'Coca Cola', 3, 7.90, false, NULL, 'Simplesmente Coca Cola'
WHERE NOT EXISTS (SELECT 1 FROM public.produto WHERE nome = 'Coca Cola');

INSERT INTO public.produto (nome, produto_categoria_id, preco, excluido, imagem, descricao) 
SELECT 'Batata Frita', 2, 6.49, false, NULL, 'Batata sequinha'
WHERE NOT EXISTS (SELECT 1 FROM public.produto WHERE nome = 'Batata Frita');

INSERT INTO public.produto (nome, produto_categoria_id, preco, excluido, imagem, descricao) 
SELECT 'Torta de Maça', 4, 529.00, false, NULL, 'Torta da vó'
WHERE NOT EXISTS (SELECT 1 FROM public.produto WHERE nome = 'Torta de Maça');



