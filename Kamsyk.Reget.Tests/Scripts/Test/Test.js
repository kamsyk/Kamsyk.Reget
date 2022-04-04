/// <reference path="../typings/jasmine/jasmine.d.ts" />
/// <reference path="../../../Kamsyk.Reget/Scripts/RegetTypeScript/Chutzpah/JasmineTest.ts" />
describe("Reget Test", function () {
    var jasmineChpTest;
    beforeEach(function () {
        jasmineChpTest = new JasmineChpTest();
    });
    it("Jasmine Chutzpah Test", function () {
        //Act
        var resNumber = jasmineChpTest.testChp();
        expect(resNumber).toBe(1);
    });
    //Stub
    it("Jasmine Chutzpah Test SpyOn", function () {
        // Arrange
        spyOn(jasmineChpTest, "getTestValue").and.returnValue(2);
        // Act
        var resNumber = jasmineChpTest.testChp();
        // Assert
        expect(resNumber).toBe(2);
    });
});
//# sourceMappingURL=test.js.map