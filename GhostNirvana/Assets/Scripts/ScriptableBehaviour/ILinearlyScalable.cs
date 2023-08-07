namespace ScriptableBehaviour {

public interface ILinearlyScalable<T> {
    public float AdditiveScale {get; set; }
    public float MultiplicativeScale {get; set; }

    public void Recompute();
}

}
