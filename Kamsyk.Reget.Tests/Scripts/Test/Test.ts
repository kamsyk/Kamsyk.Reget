/// <reference path="../typings/jasmine/jasmine.d.ts" />
/// <reference path="../../../Kamsyk.Reget/Scripts/RegetTypeScript/Chutzpah/JasmineTest.ts" />



describe("Reget Test", () => {
    var jasmineChpTest: JasmineChpTest;

    beforeEach(() => {
        jasmineChpTest = new JasmineChpTest();
    });

    it("Jasmine Chutzpah Test", () => {
        //Act
        var resNumber: number = jasmineChpTest.testChp();

        expect(resNumber).toBe(1);
    });

    //Stub
    it("Jasmine Chutzpah Test SpyOn", () => {
        // Arrange
        spyOn(jasmineChpTest, "getTestValue").and.returnValue(2);
        
        // Act
        var resNumber: number = jasmineChpTest.testChp();

        // Assert
        expect(resNumber).toBe(2);
    });
});
