Feature: Category Management
    Scenario: User can manage categories
        Given User can see categories
        Then User can create a new category Foo
        Given User try create a category named Foo
        Then The category Foo was not saved
        Given User can create a new category FooEdit
        Then User can edit a category FooEdit
        Given User can create a new category FooEdit
        Then User try edit a category with empty name
        Given User can create a new category FooDelete
        Then User can detelete a category FooDelete
        Given User create a category FooCategoryProducts with products
        Then User can not delete the category FooCategoryProducts
        
        