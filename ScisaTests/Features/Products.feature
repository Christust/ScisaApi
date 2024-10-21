Feature: product Management
    Scenario: User can manage products
        Given User can see products
        Then User can create a new product Foo
        Given User can create a new product FooEdit
        Then User can edit a product FooEdit
        Given User can create a new product FooDelete
        Then User can detelete a product FooDelete
        
        