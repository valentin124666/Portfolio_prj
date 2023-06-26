using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;

public abstract class SimpleUIPresenter <TP,TV>: SimplePresenter<TP,TV> where TP:SimpleUIPresenter<TP,TV> where TV: SimpleUIPresenterView<TP,TV>
{

    public SimpleUIPresenter(TV view) : base(view)
    {
    }
}
