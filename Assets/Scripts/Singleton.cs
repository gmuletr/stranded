using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton : MonoBehaviour
{
   //variable de tipo singleton llamada instance
   //static: no tiene que estar vinculado a un objeto 
    public static Singleton instance { get; set; }

    public string sceneToLoad;

    //Awake se ejecuta antes del start, cuando el objeto se carga en memoria
    private void Awake()
    {
        if (instance == null)
        {
            //si instance no tiene valor, le dice que su valor es la clase de este objeto
            instance = this;
            //le decimos que no se destruya al cambiar de escena
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            //si ya existe una copia, destruye la mas nueva
            Destroy(gameObject);
        }
    
    }
}
