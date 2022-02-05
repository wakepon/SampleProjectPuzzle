using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;
using Enumerable = System.Linq.Enumerable;

public class SokobanManager : MonoBehaviour
{
    [SerializeField] private float tileSize = 0.64f;
    [SerializeField] private BoxController boxPrefab;
    [SerializeField] private BoxTargetController targetPrefab;
    [SerializeField] private PlayerController player;
    [SerializeField] private Sprite groundSprite;
    [SerializeField] private Sprite wallSprite;

    private enum TileType
    {
        none,
        wall,
        ground,
        target,
        player,
        box,
    }

    private enum DirectionType
    {
        up,
        down,
        left,
        right
    }

    private string _stageString =
        "#####\n" +
        "#t.##\n" +
        "#.b.#\n" +
        "#.p.#\n" +
        "#####";

    private int _columns;
    private int _rows;
    private Vector2 centerOffset;
    private TileType[,] _tileArray;
    private List<GameObject> _tiles = new List<GameObject>();
    private List<BoxController> _boxes = new List<BoxController>();
    private List<BoxTargetController> _boxTargets = new List<BoxTargetController>();

    // Start is called before the first frame update
    void Start()
    {
        // LoadTileData(_stageString);
        // CreateStage();
    }

    public void Clear()
    {
        foreach (var tile in _tiles)
        {
            Destroy(tile.gameObject);
        }
        foreach (var box in _boxes)
        {
            Destroy(box.gameObject);
        }
        foreach (var target in _boxTargets)
        {
            Destroy(target.gameObject);
        }
        _tiles.Clear();
        _boxes.Clear();
        _boxTargets.Clear();
    }

    void LoadTileData(string tileString)
    {
        var lines = tileString.Split
        (
            new[] {'\r', '\n'},
            System.StringSplitOptions.RemoveEmptyEntries
        );

        _rows = lines.Length;
        _columns = lines[0].Length;

        _tileArray = new TileType[_columns, _rows];
        for (int y = 0; y < _rows; ++y)
        {
            var line = lines[y];
            for (int x = 0; x < _columns; ++x)
            {
                var t = CharToTileType(line[x]);
                if (t == TileType.player)
                {
                    player.Set(x, y);
                    _tileArray[x, y] = TileType.ground;
                }
                else if (t == TileType.box)
                {
                    var box = Instantiate(boxPrefab);
                    box.Set(x, y);
                    _boxes.Add(box);
                    _tileArray[x, y] = TileType.ground;
                }
                else if (t == TileType.target)
                {
                    var boxTarget = Instantiate(targetPrefab);
                    boxTarget.Set(x, y);
                    _boxTargets.Add(boxTarget);
                    _tileArray[x, y] = TileType.ground;
                }
                else
                {
                    _tileArray[x, y] = t;
                }
            }
        }
    }

    public void CreateStage(string fileName)
    {
        Clear();
        TextAsset text = Resources.Load<TextAsset>(fileName);
        LoadTileData(text.text);
        CreateStage();
    }

    void CreateStage()
    {
        centerOffset.x = _columns * tileSize / 2.0f - tileSize / 2.0f;
        centerOffset.y = _rows * tileSize / 2.0f - tileSize / 2.0f;
        for (int y = 0; y < _rows; ++y)
        {
            for (int x = 0; x < _rows; ++x)
            {
                var t = _tileArray[x, y];
                if (t == TileType.none)
                    continue;

                if (t == TileType.ground)
                {
                    var name = $"tile{y}_{x}";
                    var tile = new GameObject(name);
                    var sr = tile.AddComponent<SpriteRenderer>();
                    sr.sprite = groundSprite;
                    tile.transform.position = GetDisplayPosition(x, y);
                    _tiles.Add(tile);
                }
                else if (t == TileType.wall)
                {
                    var name = $"wall{y}_{x}";
                    var tile = new GameObject(name);
                    var sr = tile.AddComponent<SpriteRenderer>();
                    sr.sprite = wallSprite;
                    tile.transform.position = GetDisplayPosition(x, y);
                    _tiles.Add(tile);
                }
            }
        }

        foreach (var target in _boxTargets)
        {
            target.transform.position = GetDisplayPosition(target.posIndex);
        }

        foreach (var box in _boxes)
        {
            box.transform.position = GetDisplayPosition(box.posIndex);
        }

        player.transform.position = GetDisplayPosition(player.posIndex);
    }

    private bool IsValidPosition(Vector2Int pos)
    {
        if (0 <= pos.x && pos.x < _columns && 0 <= pos.y && pos.y < _rows)
        {
            return _tileArray[pos.x, pos.y] == TileType.ground;
        }

        return false;
    }

    private bool IsBox(Vector2Int pos)
    {
        return GetBoxAtPosition(pos) != null;
    }

    private BoxController GetBoxAtPosition(Vector2Int pos)
    {
        foreach (var box in _boxes)
        {
            if (box.posIndex == pos)
            {
                return box;
            }
        }

        return null;
    }

    private Vector2 GetDisplayPosition(Vector2Int pos)
    {
        return GetDisplayPosition(pos.x, pos.y);
    }

    private Vector2 GetDisplayPosition(int x, int y)
    {
        return new Vector2(
            x * tileSize - centerOffset.x,
            (_rows - y) * tileSize - centerOffset.y
        );
    }

    TileType CharToTileType(char c)
    {
        switch (c)
        {
            case '#':
                return TileType.wall;
            case '.':
                return TileType.ground;
            case 't':
                return TileType.target;
            case 'p':
                return TileType.player;
            case 'b':
                return TileType.box;
            default:
                return TileType.none;
        }
    }

    private Vector2Int GetNextPositionAlong(Vector2Int pos, DirectionType direction)
    {
        switch (direction)
        {
            case DirectionType.up:
                pos.y -= 1;
                break;
            case DirectionType.right:
                pos.x += 1;
                break;
            case DirectionType.down:
                pos.y += 1;
                break;
            case DirectionType.left:
                pos.x -= 1;
                break;
        }

        return pos;
    }

    void TryMovePlayer(DirectionType direction)
    {
        var nextPlayerPos = GetNextPositionAlong(player.posIndex, direction);

        if (!IsValidPosition(nextPlayerPos)) return;

        if (IsBox(nextPlayerPos)) // when there is a box on the tile player will move to 
        {
            var nextBlockPos = GetNextPositionAlong(nextPlayerPos, direction);

            if (IsValidPosition(nextBlockPos) && !IsBox(nextBlockPos))
            {
                //move player
                player.posIndex = nextPlayerPos;
                player.Move(GetDisplayPosition(nextPlayerPos));

                // move block
                var box = GetBoxAtPosition(nextPlayerPos);
                box.Move(GetDisplayPosition(nextBlockPos));
                box.posIndex = nextBlockPos;
            }
        }
        else // when there is no box on the tile player will move to 
        {
            //move player
            player.posIndex = nextPlayerPos;
            player.Move(GetDisplayPosition(nextPlayerPos));
        }
    }

    public void PlayerMoveOperation()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            TryMovePlayer(DirectionType.up);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            TryMovePlayer(DirectionType.right);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            TryMovePlayer(DirectionType.down);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            TryMovePlayer(DirectionType.left);
        }
    }
    
    public bool IsComplete()
    {
        int boxOnTargetCount = _boxes.Count(box => _boxTargets.Any(x=>x.posIndex == box.posIndex));
        if ( boxOnTargetCount == _boxes.Count )
        {
            return true;
        }

        return false;
    }
}