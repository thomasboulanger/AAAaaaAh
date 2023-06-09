using UnityEngine;
using UnityEditor;

public class EditModeFunctions : EditorWindow
{
    [MenuItem("Window/GraphTool")]
    public static void ShowWindow()
    {
        GetWindow<EditModeFunctions>("ExpertiseTotale");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Generate stairs"))
        {
            PlaceStepsButton();
        }
        if (GUILayout.Button("Delete stairs"))
        {
            DeleteStairs();
        }
        if (GUILayout.Button("Random volet"))
        {
            RandomVolet();
        }
        if (GUILayout.Button("VolletOpen"))
        {
            RandomVoletLocal();
        }
        if (GUILayout.Button("EnableDiscoOnSelected"))
        {
            EnableDislableDisco(true);
        }
        if (GUILayout.Button("DisableDiscoOnSelected"))
        {
            EnableDislableDisco(false);
        }
        if (GUILayout.Button("Random minaret"))
        {
            RandomMinaret();
        }
        if (GUILayout.Button("Generate scotch"))
        {
            ScotchMaker(false);
        }
        if (GUILayout.Button("scotch delete"))
        {
            ScotchMaker(true);
        }

        GUILayout.Label("Pitons", EditorStyles.boldLabel);

        if (GUILayout.Button("Generate selcted pithon"))
        {
            SelectedPithon();
        }
        if (GUILayout.Button("Generate all pithon"))
        {
            AllPithon(false);
        }
        if (GUILayout.Button("destroy all pithon"))
        {
            AllPithon(true);
        }

        GUILayout.Label("Pitons automatic regen", EditorStyles.boldLabel);

        if (GUILayout.Button("Disable automatic regen all"))
        {
            ToogleRegenPithon(true, false);
        }
        if (GUILayout.Button("Disable automatic regen selected"))
        {
            ToogleRegenPithon(true, true);
        }
        if (GUILayout.Button("toogle automatic regen all"))
        {
            ToogleRegenPithon(false,false);
        }
        if (GUILayout.Button("toogle automatic regen selected"))
        {
            ToogleRegenPithon(false, true);
        }
    }

    private void SelectedPithon()
    {
        foreach (Transform item in Selection.transforms)
        {
            PithonsManager pithonsInst;
            item.TryGetComponent<PithonsManager>(out pithonsInst);
            if (pithonsInst != null)
            {
                pithonsInst.Generate();
            }
        }
    }

    private void ToogleRegenPithon(bool disableOnly, bool selected)
    {
        if (selected)
        {
            foreach (Transform item in Selection.transforms)
            {
                PithonsManager pithons;
                item.TryGetComponent<PithonsManager>(out pithons);
                if (pithons != null)
                {
                    PithonToggle(pithons, disableOnly);
                }
                else
                {
                    foreach (Transform childWd in item)
                    {
                        PithonsManager pithonchild;
                        childWd.TryGetComponent<PithonsManager>(out pithonchild);
                        if (pithonchild != null)
                        {
                            PithonToggle(pithonchild, disableOnly);

                        }
                    }
                }
            }
        }
        else
        {
            foreach (PithonsManager item in GameObject.FindObjectsOfType<PithonsManager>())
            {
                PithonToggle(item, disableOnly);
            }
        }
    }

    void PithonToggle(PithonsManager item, bool disableOnly)
    {
        if (disableOnly)
        {
            item.automaticRegen = false;
        }
        else
        {
            item.automaticRegen = !item.automaticRegen;
        }
    }

    private void AllPithon(bool destroy)
    {
        foreach (PithonsManager item in GameObject.FindObjectsOfType<PithonsManager>())
        {
            if (destroy)
            {
                item.DestroyPreviusAssets();
            }
            else
            {
                item.Generate();
            }
        }
    }

    private void PlaceStepsButton()
    {
        RemovePlace(false);
    }

    public void DeleteStairs()
    {
        RemovePlace(true);
    }

    void RemovePlace(bool remove)
    {
        foreach (Transform item in Selection.transforms)
        {
            StairsInstance stairsInst;
            item.TryGetComponent<StairsInstance>(out stairsInst);
            if (stairsInst != null)
            {
                if (remove)
                {
                    stairsInst.RemoveSteps(false);
                }
                else
                {
                    stairsInst.PlaceSteps(false);
                }

            }
        }
    }

    void RandomVolet()
    {
        foreach (OppeningsController item in GameObject.FindObjectsOfType<OppeningsController>())
        {
            item.Randomize();
        }
    }


    void RandomVoletLocal()
    {
        foreach (Transform item in Selection.transforms)
        {
            OppeningsController windowInst;
            item.TryGetComponent<OppeningsController>(out windowInst);
            if (windowInst != null)
            {
                windowInst.Randomize();
            }
            else
            {
                foreach (Transform childWd in item)
                {
                    OppeningsController windowChildInst;
                    childWd.TryGetComponent<OppeningsController>(out windowChildInst);
                    if (windowChildInst != null)
                    {
                        windowChildInst.Randomize();
                    }
                }
            }
        }
    }

    void RandomMinaret()
    {
        foreach (HidderChanger item in GameObject.FindObjectsOfType<HidderChanger>())
        {
            item.Randomize();
        }
        foreach (LargeMinaretRandomizer item in GameObject.FindObjectsOfType<LargeMinaretRandomizer>())
        {
            item.Randomize();
        }
    }

    void EnableDislableDisco(bool enable)
    {
        foreach (Transform item in Selection.transforms)
        {
            OppeningsController windowInst;
            item.TryGetComponent<OppeningsController>(out windowInst);
            if (windowInst != null)
            {
                windowInst.Disco(enable, enable ? 1 : 0);
            }
            else
            {
                foreach (Transform childWd in item)
                {
                    OppeningsController windowChildInst;
                    childWd.TryGetComponent<OppeningsController>(out windowChildInst);
                    if (windowChildInst != null)
                    {
                        windowChildInst.Disco(enable, enable ? 1 : 0);
                    }
                }
            }
        }

    }

    void ScotchMaker(bool clear)
    {
        foreach (Transform item in Selection.transforms)
        {
            ScotchMaker scotchScript;
            item.TryGetComponent<ScotchMaker>(out scotchScript);
            if (scotchScript != null)
            {
                if (clear)
                {
                    scotchScript.ClearObjects();
                    continue;
                }
                scotchScript.ActualizeScotch();
            }
        }
    }
}
