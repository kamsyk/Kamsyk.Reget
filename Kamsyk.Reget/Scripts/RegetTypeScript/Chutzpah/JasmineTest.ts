class JasmineChpTest {
    private testValue: number = 1;

    public testChp(): number {
        return this.getTestValue();
    }

    public getTestValue(): number {
        return this.testValue;
    }
}