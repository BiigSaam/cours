extends Resource

class_name PlayerData

@export var max_health: int
@export var current_health: int
@export var speed: int
@export var jump_height: int
var initial_spawn_position: Vector2
var current_spawn_position: Vector2

signal on_death
