# Page snapshot

```yaml
- generic [ref=e4]:
  - link "Área do Vendedor" [ref=e5] [cursor=pointer]:
    - /url: /vendor/login
  - generic [ref=e6]:
    - img "ByteStore Promotion" [ref=e8]
    - generic [ref=e10]:
      - generic [ref=e11]:
        - heading "ByteStore" [level=1] [ref=e12]
        - paragraph [ref=e13]: Bem-vindo(a) de volta.
      - generic [ref=e14]:
        - generic [ref=e15]:
          - generic [ref=e16]: Email
          - generic [ref=e17]:
            - generic [ref=e19]: 
            - textbox "Email" [ref=e20]:
              - /placeholder: seuemail@exemplo.com
        - generic [ref=e21]:
          - generic [ref=e22]: Senha
          - generic [ref=e23]:
            - generic [ref=e25]: 
            - textbox "Senha" [ref=e26]:
              - /placeholder: "********"
          - link "Esqueceu a senha?" [ref=e28] [cursor=pointer]:
            - /url: /forgot-password
        - button "Login" [ref=e29] [cursor=pointer]
      - paragraph [ref=e30]:
        - text: Não tem uma conta?
        - link "Crie uma agora" [ref=e31] [cursor=pointer]:
          - /url: /register
```