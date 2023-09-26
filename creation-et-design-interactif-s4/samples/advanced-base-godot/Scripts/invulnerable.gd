extends Node

@export var character_body:CharacterBody2D

var is_invulnerable := false

func trigger(duration):
	delay(duration)
	blink()

func delay(duration):
	is_invulnerable = true
	character_body.set_collision_layer_value(9, false)
	await get_tree().create_timer(duration).timeout
	is_invulnerable = false
	character_body.set_collision_layer_value(9, true)

func blink():
	while is_invulnerable:
		await get_tree().create_timer(0.1).timeout
		character_body.modulate.a = 0.5
		await get_tree().create_timer(0.1).timeout
		character_body.modulate.a = 1
