namespace ScriptableBehaviour {

public interface ILinearlyScalable<T> {
    public T AdditiveScale {get; set; }
    public float MultiplicativeScale {get; set; }

    public void Recompute();
}

}
