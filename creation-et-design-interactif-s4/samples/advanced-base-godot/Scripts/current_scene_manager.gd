extends Node

func _ready():
	process_mode = Node.PROCESS_MODE_ALWAYS

func _input(event):
	if event is InputEventKey and event.pressed:
		if event.keycode == KEY_R:
			get_tree().reload_current_scene()
	if event.is_action_pressed("Pause-Resume"):

		get_tree().paused = !get_tree().paused
